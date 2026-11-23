/*
 * Archivo:    backend/src/Acompana.Application/Analytics/SalesQuery.cs
 * Qué hace:   Ventas por paquete, sede y mes + KPI.
 * Recibe:     from, to, branch?
 * Procesa:    Lee analytics.mv_sales_by_package_branch_month (almacén analítico separado de la
 *               operación).
 * Devuelve:   DTO de la sección correspondiente.
 * Se conecta: ← GET /api/analytics/... · → analytics.mv_sales_by_package_branch_month
 * Reglas:     RF-32
 * Estado:     Completo
 */

using Acompana.Application.Common;
using Microsoft.EntityFrameworkCore;

namespace Acompana.Application.Analytics;

public sealed class SalesQuery
{
    private readonly IAppDbContext _db;
    private readonly ICurrentUser _user;
    private readonly IClock _clock;
    public SalesQuery(IAppDbContext db, ICurrentUser user, IClock clock) { _db = db; _user = user; _clock = clock; }

    private class Pkg { public string Name { get; set; } = ""; public decimal Total { get; set; } public int Count { get; set; } }
    private class Mon { public string Month { get; set; } = ""; public decimal Total { get; set; } }
    private class Br { public string Name { get; set; } = ""; public decimal Total { get; set; } public int Count { get; set; } }
    private class Stage { public string From { get; set; } = ""; public string To { get; set; } = ""; public decimal Hours { get; set; } }

    public async Task<SalesResponse> HandleAsync(DateOnly? start, DateOnly? end, long? branch, CancellationToken ct)
    {
        var from = start ?? new DateOnly(_clock.Today.Year, _clock.Today.Month, 1).AddMonths(-11);
        var to = end ?? _clock.Today;
        var branchId = _user.SeesAllBranches ? branch : _user.BranchId;
        var f = from.ToDateTime(TimeOnly.MinValue); var t = to.ToDateTime(TimeOnly.MinValue);
        var b = branchId ?? 0L;

        const string where = @"WHERE month >= {0} AND month <= {1} AND ({2} = 0 OR branch_id = {2})";
        var byPackage = await _db.SqlQueryRaw<Pkg>($@"SELECT package_name AS ""Name"", sum(total_crc) AS ""Total"", sum(services_count)::int AS ""Count""
            FROM analytics.mv_sales_by_package_branch_month {where} GROUP BY 1 ORDER BY 2 DESC", f, t, b).ToListAsync(ct);
        var byMonth = await _db.SqlQueryRaw<Mon>($@"SELECT to_char(month, 'YYYY-MM') AS ""Month"", sum(total_crc) AS ""Total""
            FROM analytics.mv_sales_by_package_branch_month {where} GROUP BY 1 ORDER BY 1", f, t, b).ToListAsync(ct);
        var byBranch = await _db.SqlQueryRaw<Br>($@"SELECT branch_name AS ""Name"", sum(total_crc) AS ""Total"", sum(services_count)::int AS ""Count""
            FROM analytics.mv_sales_by_package_branch_month {where} GROUP BY 1 ORDER BY 2 DESC", f, t, b).ToListAsync(ct);
        var stages = await _db.SqlQueryRaw<Stage>($@"SELECT from_status AS ""From"", to_status AS ""To"", (sum(avg_hours * samples) / NULLIF(sum(samples), 0))::numeric(10,2) AS ""Hours""
            FROM analytics.mv_stage_times {where} GROUP BY 1, 2", f, t, b).ToListAsync(ct);

        var monthStart = new DateTimeOffset(new DateTime(_clock.Today.Year, _clock.Today.Month, 1), TimeSpan.FromHours(-6)).ToUniversalTime();
        var servicesQ = _db.Services.AsNoTracking().ForUser(_user, branch);
        var thisMonth = await servicesQ.CountAsync(s => s.ScheduledAt >= monthStart && s.Status != Domain.Enums.ServiceStatus.Cancelled, ct);
        var sold = byPackage.Sum(p => p.Count);
        var avgTicket = sold == 0 ? 0 : Math.Round(byPackage.Sum(p => p.Total) / sold, 0);
        var fromUtc = new DateTimeOffset(f, TimeSpan.FromHours(-6)).ToUniversalTime(); var toUtc = new DateTimeOffset(t.AddDays(1), TimeSpan.FromHours(-6)).ToUniversalTime();
        var paid = servicesQ.Where(s => s.ScheduledAt >= fromUtc && s.ScheduledAt < toUtc && (s.Status == Domain.Enums.ServiceStatus.Paid || s.Status == Domain.Enums.ServiceStatus.InProgress || s.Status == Domain.Enums.ServiceStatus.Completed));
        var paidTotal = await paid.CountAsync(ct);
        var paidInsurer = await paid.CountAsync(s => s.Payer == Domain.Enums.Payer.Insurer, ct);
        var hours = stages.Where(s => s.From is "Registered" or "Invoicing").Sum(s => s.Hours);

        return new SalesResponse(
            new KpiDto(thisMonth, avgTicket, paidTotal == 0 ? 0 : Math.Round(100m * paidInsurer / paidTotal, 1), Math.Round(hours, 1)),
            new SalesDto(byPackage.Select(p => new SalesByPackage(p.Name, p.Total, p.Count)).ToList(),
                byMonth.Select(m => new SalesByMonth(m.Month, m.Total)).ToList(),
                byBranch.Select(x => new SalesByBranch(x.Name, x.Total, x.Count)).ToList()));
    }
}
