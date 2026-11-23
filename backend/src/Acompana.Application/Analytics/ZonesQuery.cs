/*
 * Archivo:    backend/src/Acompana.Application/Analytics/ZonesQuery.cs
 * Qué hace:   Servicios por zona y mes (mapa de calor).
 * Recibe:     year
 * Procesa:    Lee analytics.mv_services_by_zone_month (almacén analítico separado de la operación).
 * Devuelve:   DTO de la sección correspondiente.
 * Se conecta: ← GET /api/analytics/... · → analytics.mv_services_by_zone_month
 * Reglas:     RF-34
 * Estado:     Completo
 */

using Acompana.Application.Common;
using Microsoft.EntityFrameworkCore;

namespace Acompana.Application.Analytics;

public sealed class ZonesQuery
{
    private readonly IAppDbContext _db;
    private readonly IClock _clock;
    public ZonesQuery(IAppDbContext db, IClock clock) { _db = db; _clock = clock; }

    private class Cell { public string Zone { get; set; } = ""; public string Month { get; set; } = ""; public int Count { get; set; } }

    /// <summary>Si year es null, devuelve los últimos 12 meses.</summary>
    public async Task<ZonesDto> HandleAsync(int? year, CancellationToken ct)
    {
        var from = year is null ? new DateOnly(_clock.Today.Year, _clock.Today.Month, 1).AddMonths(-11) : new DateOnly(year.Value, 1, 1);
        var to = year is null ? _clock.Today : new DateOnly(year.Value, 12, 31);
        var rows = await _db.SqlQueryRaw<Cell>(@"SELECT zone AS ""Zone"", to_char(month, 'YYYY-MM') AS ""Month"", services_count AS ""Count""
            FROM analytics.mv_services_by_zone_month WHERE month >= {0} AND month <= {1} ORDER BY month, zone", from.ToDateTime(TimeOnly.MinValue), to.ToDateTime(TimeOnly.MinValue)).ToListAsync(ct);
        return new ZonesDto(rows.Select(r => new ZoneCellDto(r.Zone, r.Month, r.Count)).ToList());
    }
}
