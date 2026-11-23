/*
 * Archivo:    backend/src/Acompana.Application/Logistics/CremationQueueQuery.cs
 * Qué hace:   Tablero kanban del crematorio central.
 * Recibe:     nada (crematorio único en San José).
 * Procesa:    Cremaciones no entregadas + entregadas hoy, agrupadas por estado y ordenadas por
 *               QueuePosition.
 * Devuelve:   { waiting: CremationCardDto[], inOven: [], packed: [], delivered: [] }.
 * Se conecta: ← GET /api/cremations
 * Reglas:     RF-21
 * Estado:     Completo
 */

using Acompana.Application.Common;
using Acompana.Domain.Enums;
using Microsoft.EntityFrameworkCore;

namespace Acompana.Application.Logistics;

public sealed class CremationQueueQuery
{
    private readonly IAppDbContext _db;
    private readonly IClock _clock;
    public CremationQueueQuery(IAppDbContext db, IClock clock) { _db = db; _clock = clock; }

    public async Task<CremationBoardDto> HandleAsync(CancellationToken ct)
    {
        var since = _clock.Now.AddHours(-48);
        var rows = await (from c in _db.Cremations.AsNoTracking()
                          join s in _db.Services.AsNoTracking().Include(x => x.Deceased) on c.ServiceId equals s.Id
                          join e in _db.Employees.AsNoTracking() on c.OperatorId equals e.Id into ee
                          from e in ee.DefaultIfEmpty()
                          where c.Status != CremationStatus.Delivered || c.DeliveredAt >= since
                          orderby c.QueuePosition
                          select new { c, s.Code, Name = s.Deceased!.FullName, Op = e != null ? e.FullName : null }).ToListAsync(ct);
        var cards = rows.Select(r => Map(r.c, r.Code, r.Name, r.Op)).ToList();
        return new CremationBoardDto(Of(cards, "Waiting"), Of(cards, "InOven"), Of(cards, "Packed"), Of(cards, "Delivered"));
    }

    private static IReadOnlyList<CremationCardDto> Of(List<CremationCardDto> l, string s) => l.Where(c => c.Status == s).ToList();

    public static CremationCardDto Map(Domain.Logistics.Cremation c, string code, string name, string? op) =>
        new(c.Id, code, name, c.Status.ToString(), c.QueuePosition, op, c.WaitingAt, c.InOvenAt, c.PackedAt, c.DeliveredAt, c.ReceivedBy);
}
