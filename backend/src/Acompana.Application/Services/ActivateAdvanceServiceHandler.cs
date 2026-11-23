/*
 * Archivo:    backend/src/Acompana.Application/Services/ActivateAdvanceServiceHandler.cs
 * Qué hace:   Activa un servicio anticipado cuando ocurre el fallecimiento.
 * Recibe:     id + { deathDate, scheduledAt }.
 * Procesa:    Verifica Kind = Advance y Status = Advance; fija DeathDate; ChangeStatus(Registered);
 *               conserva paquete y datos (no se repiten).
 * Devuelve:   ServiceDetailDto.
 * Se conecta: ← POST /api/services/{id}/activate
 * Reglas:     RF-02
 * Estado:     Completo
 *
 * Contrato:
 *   Task<ServiceDetailDto> HandleAsync(Guid id, ActivateRequest req, CancellationToken ct)
 *     Recibe:     id, fecha de defunción, fecha del servicio
 *     Procesa:    ver arriba
 *     Devuelve:   detalle
 *     Errores:    422 'El servicio no es anticipado o ya fue activado.'
 */

using Acompana.Application.Common;
using Acompana.Domain.Common;
using Acompana.Domain.Enums;
using Microsoft.EntityFrameworkCore;

namespace Acompana.Application.Services;

public sealed class ActivateAdvanceServiceHandler
{
    private readonly IAppDbContext _db;
    private readonly ICurrentUser _user;
    private readonly IClock _clock;
    private readonly IOpsNotifier _notifier;
    private readonly GetServiceDetailQuery _detail;

    public ActivateAdvanceServiceHandler(IAppDbContext db, ICurrentUser user, IClock clock, IOpsNotifier notifier, GetServiceDetailQuery detail)
    { _db = db; _user = user; _clock = clock; _notifier = notifier; _detail = detail; }

    public async Task<ServiceDetailDto> HandleAsync(Guid id, ActivateRequest req, CancellationToken ct)
    {
        var s = await _db.Services.Include(x => x.Deceased).Include(x => x.Lines).Include(x => x.DeathCertificate).FirstOrDefaultAsync(x => x.Id == id, ct);
        if (s is null || (!_user.SeesAllBranches && s.BranchId != _user.BranchId)) throw new NotFoundException("El servicio no existe.");
        if (s.Kind != ServiceKind.Advance || s.Status != ServiceStatus.Advance)
            throw new DomainException("El servicio no es anticipado o ya fue activado.", "not-advance");
        if (req.DeathDate > _clock.Today) throw new ValidationException("deathDate", "La fecha de defunción no puede ser futura.");
        if (req.DeathDate < s.Deceased!.BirthDate) throw new ValidationException("deathDate", "La defunción no puede ser anterior al nacimiento.");

        var newAt = req.ScheduledAt.ToUniversalTime();
        var shift = newAt - s.ScheduledAt;
        s.Deceased.DeathDate = req.DeathDate;
        s.ScheduledAt = newAt;
        foreach (var l in s.Lines.Where(l => l.DueAt != null)) l.DueAt = l.DueAt!.Value + shift;
        var booking = await _db.ChapelBookings.FirstOrDefaultAsync(b => b.ServiceId == id, ct);
        if (booking is not null) { booking.StartsAt += shift; booking.EndsAt += shift; }
        s.ChangeStatus(ServiceStatus.Registered, _user.DisplayName, _clock.Now);
        try { await _db.SaveChangesAsync(ct); }
        catch (DbUpdateException ex) when (ex is not DbUpdateConcurrencyException)
        {
            throw new ConflictException("La capilla ya está reservada en la nueva fecha.");
        }
        await _notifier.ServiceUpdated(s.Id, s.BranchId);
        return await _detail.HandleAsync(s.Id, ct);
    }
}
