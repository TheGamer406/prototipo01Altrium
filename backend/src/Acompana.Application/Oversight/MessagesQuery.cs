/*
 * Archivo:    backend/src/Acompana.Application/Oversight/MessagesQuery.cs
 * Qué hace:   Correos simulados enviados.
 * Recibe:     serviceId?, kind?, page.
 * Procesa:    Consulta outbound_messages.
 * Devuelve:   PagedResult<MessageDto>.
 * Se conecta: ← GET /api/messages
 * Reglas:     RF-12 · RF-15 · RF-35
 * Estado:     Completo
 */

using Acompana.Application.Common;
using Acompana.Domain.Enums;
using Microsoft.EntityFrameworkCore;

namespace Acompana.Application.Oversight;

public sealed class MessagesQuery
{
    private readonly IAppDbContext _db;
    private readonly ICurrentUser _user;
    public MessagesQuery(IAppDbContext db, ICurrentUser user) { _db = db; _user = user; }

    public async Task<PagedResult<MessageDto>> HandleAsync(Guid? serviceId, string? kind, int page, int pageSize, CancellationToken ct)
    {
        page = Math.Max(1, page); pageSize = pageSize is < 1 or > 100 ? 25 : pageSize;
        var q = from m in _db.OutboundMessages.AsNoTracking()
                join s in _db.Services.AsNoTracking() on m.ServiceId equals s.Id into ss
                from s in ss.DefaultIfEmpty()
                select new { m, Code = s != null ? s.Code : null, Branch = s != null ? (long?)s.BranchId : null };
        if (!_user.SeesAllBranches) q = q.Where(x => x.Branch == _user.BranchId);
        if (serviceId is not null) q = q.Where(x => x.m.ServiceId == serviceId);
        if (Enum.TryParse<MessageKind>(kind, true, out var k)) q = q.Where(x => x.m.Kind == k);
        var total = await q.CountAsync(ct);
        var rows = await q.OrderByDescending(x => x.m.CreatedAt).Skip((page - 1) * pageSize).Take(pageSize).ToListAsync(ct);
        return new PagedResult<MessageDto>(rows.Select(x => new MessageDto(x.m.Id, x.m.ToAddress, x.m.Kind.ToString(), x.m.Subject, x.m.Body, x.Code, x.m.CreatedAt)).ToList(), page, pageSize, total);
    }
}
