/*
 * Archivo:    backend/src/Acompana.Application/Oversight/AuditQuery.cs
 * Qué hace:   Bitácora filtrable.
 * Recibe:     entity?, entityId?, from?, to?, actor?, page.
 * Procesa:    Filtra audit_log; solo Management.
 * Devuelve:   PagedResult<AuditEntryDto>.
 * Se conecta: ← GET /api/audit · pestaña Bitácora del servicio
 * Reglas:     RF-31
 * Estado:     Completo
 */

using Acompana.Application.Common;
using Acompana.Domain.Enums;
using Microsoft.EntityFrameworkCore;

namespace Acompana.Application.Oversight;

public sealed class AuditQuery
{
    private readonly IAppDbContext _db;
    private readonly ICurrentUser _user;
    public AuditQuery(IAppDbContext db, ICurrentUser user) { _db = db; _user = user; }

    public async Task<PagedResult<AuditEntryDto>> HandleAsync(string? entity, string? entityId, DateTimeOffset? start, DateTimeOffset? end,
        string? actor, Guid? serviceId, int page, int pageSize, CancellationToken ct)
    {
        if (_user.Role != DemoRole.Management) throw new ForbiddenException("Solo Gerencia puede consultar la bitácora.");
        page = Math.Max(1, page); pageSize = pageSize is < 1 or > 100 ? 25 : pageSize;
        var q = _db.AuditLog.AsNoTracking().AsQueryable();
        if (!string.IsNullOrWhiteSpace(entity)) q = q.Where(a => a.Entity == entity);
        if (!string.IsNullOrWhiteSpace(entityId)) q = q.Where(a => a.EntityId == entityId);
        if (serviceId is not null) q = q.Where(a => a.ServiceId == serviceId);
        if (start is not null) q = q.Where(a => a.At >= start);
        if (end is not null) q = q.Where(a => a.At <= end);
        if (!string.IsNullOrWhiteSpace(actor)) { var t = actor.ToLower(); q = q.Where(a => a.ActorName.ToLower().Contains(t)); }
        var total = await q.CountAsync(ct);
        var rows = await (from a in q.OrderByDescending(a => a.At).Skip((page - 1) * pageSize).Take(pageSize)
                          join b in _db.Branches.AsNoTracking() on a.BranchId equals b.Id into bb
                          from b in bb.DefaultIfEmpty()
                          select new { a, Branch = b != null ? b.Name : null }).ToListAsync(ct);
        return new PagedResult<AuditEntryDto>(rows.Select(r => new AuditEntryDto(r.a.Id, r.a.At, r.a.ActorRole, r.a.ActorName, r.Branch, r.a.Entity, r.a.EntityId,
            r.a.Action.ToString(), r.a.OldValues, r.a.NewValues)).ToList(), page, pageSize, total);
    }
}
