/*
 * Archivo:    backend/src/Acompana.Application/Logistics/AdvanceCremationHandler.cs
 * Qué hace:   Mueve una cremación al siguiente estado.
 * Recibe:     cremationId + { operatorId, receivedBy? }.
 * Procesa:    Cremation.Advance(...); al entregar, FamilyNotifier (cenizas entregadas); notifica
 *               CremationMoved.
 * Devuelve:   CremationCardDto actualizado.
 * Se conecta: ← POST /api/cremations/{id}/advance
 * Reglas:     RF-21 · RF-35
 * Estado:     Completo
 */

using Acompana.Application.Common;
using Acompana.Application.Services;
using Acompana.Domain.Enums;
using Microsoft.EntityFrameworkCore;

namespace Acompana.Application.Logistics;

public sealed class AdvanceCremationHandler
{
    private readonly IAppDbContext _db;
    private readonly ICurrentUser _user;
    private readonly IClock _clock;
    private readonly IOpsNotifier _notifier;
    private readonly FamilyNotifier _family;
    public AdvanceCremationHandler(IAppDbContext db, ICurrentUser user, IClock clock, IOpsNotifier notifier, FamilyNotifier family)
    { _db = db; _user = user; _clock = clock; _notifier = notifier; _family = family; }

    public async Task<CremationCardDto> HandleAsync(long id, AdvanceCremationRequest req, CancellationToken ct)
    {
        if (_user.Role is not (DemoRole.CrematoriumOperator or DemoRole.Management)) throw new ForbiddenException("Solo el operador del crematorio mueve las tarjetas.");
        var c = await _db.Cremations.FirstOrDefaultAsync(x => x.Id == id, ct) ?? throw new NotFoundException("La cremación no existe.");
        var s = await _db.Services.Include(x => x.Deceased).Include(x => x.FamilyContact).FirstAsync(x => x.Id == c.ServiceId, ct);
        c.Advance(req.OperatorId, _clock.Now, req.ReceivedBy);
        await _db.SaveChangesAsync(ct);
        if (c.Status == CremationStatus.Delivered) await _family.AshesDeliveredAsync(s, c.ReceivedBy ?? "", ct);
        await _notifier.CremationMoved(c.Id, c.Status.ToString());
        var op = await _db.Employees.AsNoTracking().Where(e => e.Id == c.OperatorId).Select(e => e.FullName).FirstOrDefaultAsync(ct);
        return CremationQueueQuery.Map(c, s.Code, s.Deceased!.FullName, op);
    }
}
