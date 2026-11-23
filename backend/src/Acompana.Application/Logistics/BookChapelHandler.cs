/*
 * Archivo:    backend/src/Acompana.Application/Logistics/BookChapelHandler.cs
 * Qué hace:   Reserva una capilla sin traslapes.
 * Recibe:     BookChapelRequest.
 * Procesa:    Inserta ChapelBooking; si PostgreSQL lanza 23P01 (exclusion_violation) busca la reserva
 *               que choca y arma el mensaje.
 * Devuelve:   Id de la reserva.
 * Se conecta: ← POST /api/chapel-bookings, CreateServiceHandler · probado en ChapelOverlapTests
 * Reglas:     RF-20
 * Estado:     Completo
 *
 * Contrato:
 *   Task<Guid> HandleAsync(BookChapelRequest req, CancellationToken ct)
 *     Recibe:     capilla, servicio, rango
 *     Procesa:    insert + captura de exclusión
 *     Devuelve:   id
 *     Errores:    409 'La capilla {nombre} está reservada para {código} de {hh:mm} a {hh:mm}.'
 */

using Acompana.Application.Common;
using Acompana.Domain.Logistics;
using Microsoft.EntityFrameworkCore;

namespace Acompana.Application.Logistics;

public sealed class BookChapelHandler
{
    private readonly IAppDbContext _db;
    public BookChapelHandler(IAppDbContext db) => _db = db;

    public async Task<Guid> HandleAsync(BookChapelRequest req, CancellationToken ct)
    {
        if (req.To <= req.From) throw new ValidationException("to", "La hora final debe ser posterior a la inicial.");
        var chapel = await _db.Chapels.AsNoTracking().FirstOrDefaultAsync(c => c.Id == req.ChapelId, ct)
                     ?? throw new NotFoundException("La capilla no existe.");
        var start = req.From.ToUniversalTime(); var end = req.To.ToUniversalTime();

        var clash = await FindClashAsync(chapel.Id, start, end, req.ServiceId, ct);
        if (clash is not null) throw new ConflictException(clash);

        var booking = new ChapelBooking { ChapelId = chapel.Id, ServiceId = req.ServiceId, StartsAt = start, EndsAt = end };
        _db.ChapelBookings.Add(booking);
        try { await _db.SaveChangesAsync(ct); }
        catch (DbUpdateException)
        {
            // la restricción EXCLUDE de PostgreSQL es la garantía final (carrera entre dos usuarios)
            var late = await FindClashAsync(chapel.Id, start, end, req.ServiceId, ct);
            throw new ConflictException(late ?? $"La capilla {chapel.Name} ya está reservada en ese horario.");
        }
        return booking.Id;
    }

    /// <summary>Mensaje 'La capilla X está reservada para SRV-… de hh:mm a hh:mm.' o null si está libre.</summary>
    public async Task<string?> FindClashAsync(long chapelId, DateTimeOffset start, DateTimeOffset end, Guid? ignoreService, CancellationToken ct)
    {
        var other = await (from b in _db.ChapelBookings.AsNoTracking()
                           join s in _db.Services.AsNoTracking() on b.ServiceId equals s.Id
                           join c in _db.Chapels.AsNoTracking() on b.ChapelId equals c.Id
                           where b.ChapelId == chapelId && b.StartsAt < end && start < b.EndsAt && (ignoreService == null || b.ServiceId != ignoreService)
                           orderby b.StartsAt
                           select new { c.Name, s.Code, b.StartsAt, b.EndsAt }).FirstOrDefaultAsync(ct);
        if (other is null) return null;
        string Hm(DateTimeOffset d) => d.ToOffset(TimeSpan.FromHours(-6)).ToString("dd/MM HH:mm");
        return $"La capilla {other.Name} está reservada para {other.Code} de {Hm(other.StartsAt)} a {Hm(other.EndsAt)}.";
    }
}
