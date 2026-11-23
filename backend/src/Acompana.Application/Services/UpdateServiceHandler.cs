/*
 * Archivo:    backend/src/Acompana.Application/Services/UpdateServiceHandler.cs
 * Qué hace:   Edita datos generales del servicio (familia, velación, destino, fecha, pagador).
 * Recibe:     id + UpdateServiceRequest (mismos campos editables que la creación, sin líneas) +
 *               Version.
 * Procesa:    Carga, compara Version (concurrencia optimista), aplica cambios; no permite editar si
 *               Status ≥ Paid salvo fecha y dirección.
 * Devuelve:   ServiceDetailDto actualizado.
 * Se conecta: ← PUT /api/services/{id} · → AuditInterceptor registra antes/después
 * Reglas:     RF-01 · RF-07 · RF-31
 * Estado:     Completo
 *
 * Contrato:
 *   Task<ServiceDetailDto> HandleAsync(Guid id, UpdateServiceRequest req, CancellationToken ct)
 *     Recibe:     id, cambios, versión
 *     Procesa:    ver arriba
 *     Devuelve:   detalle
 *     Errores:    404 · 409 'Otro usuario modificó este servicio; recargue.'
 */

using Acompana.Application.Common;
using Acompana.Domain.Enums;
using Microsoft.EntityFrameworkCore;

namespace Acompana.Application.Services;

public sealed class UpdateServiceHandler
{
    private readonly IAppDbContext _db;
    private readonly ICurrentUser _user;
    private readonly IClock _clock;
    private readonly IOpsNotifier _notifier;
    private readonly GetServiceDetailQuery _detail;

    public UpdateServiceHandler(IAppDbContext db, ICurrentUser user, IClock clock, IOpsNotifier notifier, GetServiceDetailQuery detail)
    { _db = db; _user = user; _clock = clock; _notifier = notifier; _detail = detail; }

    public async Task<ServiceDetailDto> HandleAsync(Guid id, UpdateServiceRequest req, CancellationToken ct)
    {
        var s = await _db.Services.Include(x => x.FamilyContact).Include(x => x.Lines).FirstOrDefaultAsync(x => x.Id == id, ct);
        if (s is null || (!_user.SeesAllBranches && s.BranchId != _user.BranchId)) throw new NotFoundException("El servicio no existe.");
        if (s.Version != req.Version) throw new ConflictException("Otro usuario modificó este servicio; recargue.");

        var locked = s.Status >= ServiceStatus.Paid && s.Status != ServiceStatus.Cancelled;
        var newAt = req.ScheduledAt.ToUniversalTime();
        if (newAt != s.ScheduledAt)
        {
            var shift = newAt - s.ScheduledAt;
            s.ScheduledAt = newAt;
            foreach (var l in s.Lines.Where(l => l.DueAt != null)) l.DueAt = l.DueAt!.Value + shift;
            var booking = await _db.ChapelBookings.FirstOrDefaultAsync(b => b.ServiceId == id, ct);
            if (booking is not null) { booking.StartsAt += shift; booking.EndsAt += shift; }
        }
        if (s.WakePlace != WakePlace.Chapel) s.WakeAddress = req.WakeAddress?.Trim();

        if (!locked)
        {
            var f = s.FamilyContact!;
            f.FullName = req.Family.FullName.Trim(); f.NationalId = req.Family.NationalId; f.Relationship = req.Family.Relationship;
            f.Phone = req.Family.Phone; f.Email = req.Family.Email.Trim();
            f.SetConsent(req.Family.InsuranceProspectConsent, _clock.Now);
            s.FinalDestination = req.FinalDestination;
            s.CemeteryName = req.FinalDestination == FinalDestination.Cemetery ? req.CemeteryName?.Trim() : null;
            s.Payer = req.Payer;
            s.InsurerId = req.Payer == Payer.Insurer ? req.InsurerId : null;
            s.PolicyNumber = req.Payer == Payer.Insurer ? req.PolicyNumber?.Trim() : null;
        }
        try { await _db.SaveChangesAsync(ct); }
        catch (DbUpdateException ex) when (ex is not DbUpdateConcurrencyException)
        {
            throw new ConflictException("La capilla ya está reservada en el nuevo horario.");
        }
        await _notifier.ServiceUpdated(s.Id, s.BranchId);
        return await _detail.HandleAsync(s.Id, ct);
    }
}
