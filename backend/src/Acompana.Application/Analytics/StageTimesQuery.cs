/*
 * Archivo:    backend/src/Acompana.Application/Analytics/StageTimesQuery.cs
 * Qué hace:   Tiempo promedio entre estados y etapa más lenta por sede.
 * Recibe:     from, to
 * Procesa:    Lee analytics.mv_stage_times (almacén analítico separado de la operación).
 * Devuelve:   DTO de la sección correspondiente.
 * Se conecta: ← GET /api/analytics/... · → analytics.mv_stage_times
 * Reglas:     RF-33
 * Estado:     Completo
 */

using Acompana.Application.Common;
using Microsoft.EntityFrameworkCore;

namespace Acompana.Application.Analytics;

public sealed class StageTimesQuery
{
    private readonly IAppDbContext _db;
    private readonly ICurrentUser _user;
    private readonly IClock _clock;
    public StageTimesQuery(IAppDbContext db, ICurrentUser user, IClock clock) { _db = db; _user = user; _clock = clock; }

    private class Stage { public string From { get; set; } = ""; public string To { get; set; } = ""; public decimal Hours { get; set; } }
    private class BranchStage { public string Branch { get; set; } = ""; public string From { get; set; } = ""; public string To { get; set; } = ""; public decimal Hours { get; set; } }

    public async Task<StageTimesDto> HandleAsync(DateOnly? start, DateOnly? end, CancellationToken ct)
    {
        var from = start ?? new DateOnly(_clock.Today.Year, _clock.Today.Month, 1).AddMonths(-11);
        var to = end ?? _clock.Today;
        long b = _user.SeesAllBranches ? 0 : _user.BranchId ?? -1;

        var stages = await _db.SqlQueryRaw<Stage>(@"SELECT from_status AS ""from"", to_status AS ""to"", (sum(avg_hours * samples) / NULLIF(sum(samples), 0))::numeric(10,2) AS ""hours""
            FROM analytics.mv_stage_times WHERE month >= {0} AND month <= {1} AND ({2} = 0 OR branch_id = {2}) GROUP BY 1, 2", from, to, b).ToListAsync(ct);
        var perBranch = await _db.SqlQueryRaw<BranchStage>(@"SELECT branch_name AS ""branch"", from_status AS ""from"", to_status AS ""to"", (sum(avg_hours * samples) / NULLIF(sum(samples), 0))::numeric(10,2) AS ""hours""
            FROM analytics.mv_stage_times WHERE month >= {0} AND month <= {1} AND ({2} = 0 OR branch_id = {2}) GROUP BY 1, 2, 3", from, to, b).ToListAsync(ct);

        var order = new[] { "Registered", "Invoicing", "Paid", "InProgress" };
        var slowest = perBranch.Where(x => x.From is "Registered" or "Invoicing").GroupBy(x => x.Branch).Select(g => g.OrderByDescending(x => x.Hours).First())
            .OrderByDescending(x => x.Hours).Take(10)
            .Select(x => new SlowestStageDto(x.Branch, $"{x.From} → {x.To}", x.Hours)).ToList();
        return new StageTimesDto(stages.OrderBy(s => Array.IndexOf(order, s.From)).Select(s => new StageDto(s.From, s.To, s.Hours)).ToList(), slowest);
    }
}
