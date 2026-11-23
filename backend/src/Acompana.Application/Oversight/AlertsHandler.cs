/*
 * Archivo:    backend/src/Acompana.Application/Oversight/AlertsHandler.cs
 * Qué hace:   Lista y marca como vistas las alertas.
 * Recibe:     branch? · alertId para ack.
 * Procesa:    BranchScope; ack fija AcknowledgedAt.
 * Devuelve:   AlertDto[] · 204.
 * Se conecta: ← GET /api/alerts, POST /api/alerts/{id}/ack · campana de la barra superior
 * Reglas:     RF-17 · RF-24 · RF-25 · RF-27
 * Estado:     Completo
 */

using Acompana.Application.Common;
using Microsoft.EntityFrameworkCore;

namespace Acompana.Application.Oversight;

public sealed class AlertsHandler
{
    private readonly IAppDbContext _db;
    private readonly ICurrentUser _user;
    private readonly IClock _clock;
    public AlertsHandler(IAppDbContext db, ICurrentUser user, IClock clock) { _db = db; _user = user; _clock = clock; }

    public async Task<IReadOnlyList<AlertDto>> ListAsync(long? branch, bool includeAcknowledged, CancellationToken ct)
    {
        var q = _db.Alerts.AsNoTracking().ForUser(_user, branch);
        if (!includeAcknowledged) q = q.Where(a => a.AcknowledgedAt == null);
        var rows = await (from a in q
                          join b in _db.Branches.AsNoTracking() on a.BranchId equals b.Id
                          join s in _db.Services.AsNoTracking() on a.ServiceId equals s.Id into ss
                          from s in ss.DefaultIfEmpty()
                          orderby a.CreatedAt descending
                          select new { a.Id, a.Type, Branch = b.Name, Code = s != null ? s.Code : null, a.Message, a.CreatedAt, a.AcknowledgedAt }).Take(100).ToListAsync(ct);
        return rows.Select(r => new AlertDto(r.Id, r.Type.ToString(), r.Branch, r.Code, r.Message, r.CreatedAt, r.AcknowledgedAt)).ToList();
    }

    public async Task AcknowledgeAsync(long id, CancellationToken ct)
    {
        var a = await _db.Alerts.FirstOrDefaultAsync(x => x.Id == id, ct) ?? throw new NotFoundException("La alerta no existe.");
        if (!_user.SeesAllBranches && a.BranchId != _user.BranchId) throw new NotFoundException("La alerta no existe.");
        a.Acknowledge(_clock.Now);
        await _db.SaveChangesAsync(ct);
    }
}
