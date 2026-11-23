/*
 * Archivo:    backend/src/Acompana.Application/Admin/RefreshAnalyticsHandler.cs
 * Qué hace:   Refresca las 4 vistas materializadas.
 * Recibe:     nada.
 * Procesa:    REFRESH MATERIALIZED VIEW para cada vista de analytics.
 * Devuelve:   { refreshedAt }.
 * Se conecta: ← POST /api/admin/refresh-analytics, ResetDemoHandler, seed
 * Reglas:     RF-32 a RF-34
 * Estado:     Completo
 */

using Acompana.Application.Common;

namespace Acompana.Application.Admin;

public sealed class RefreshAnalyticsHandler
{
    public const string RefreshAllSql = @"REFRESH MATERIALIZED VIEW analytics.mv_sales_by_package_branch_month;
REFRESH MATERIALIZED VIEW analytics.mv_stage_times;
REFRESH MATERIALIZED VIEW analytics.mv_services_by_zone_month;
REFRESH MATERIALIZED VIEW analytics.mv_branch_snapshot;";

    private readonly IAppDbContext _db;
    private readonly IClock _clock;
    public RefreshAnalyticsHandler(IAppDbContext db, IClock clock) { _db = db; _clock = clock; }

    public async Task<object> HandleAsync(CancellationToken ct)
    {
        await _db.ExecuteSqlRawAsync(RefreshAllSql, ct);
        return new { refreshedAt = _clock.Now };
    }

    public Task RefreshSnapshotAsync(CancellationToken ct) =>
        _db.ExecuteSqlRawAsync("REFRESH MATERIALIZED VIEW analytics.mv_branch_snapshot;", ct);
}
