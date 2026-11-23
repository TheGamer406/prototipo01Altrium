/*
 * Archivo:    backend/src/Acompana.Infrastructure/Persistence/AnalyticsViews.cs
 * Qué hace:   SQL de las 4 vistas materializadas del esquema analytics.
 * Recibe:     —
 * Procesa:    Constantes SQL que usa la migración: mv_sales_by_package_branch_month, mv_stage_times,
 *               mv_services_by_zone_month, mv_branch_snapshot; y RefreshAllSql.
 * Devuelve:   Strings SQL.
 * Se conecta: ← migración inicial, RefreshAnalyticsHandler
 * Reglas:     RF-29 · RF-32 · RF-33 · RF-34
 * Estado:     Completo
 */

namespace Acompana.Infrastructure.Persistence;

public static class AnalyticsViews
{

    public const string CreateSchema = "CREATE SCHEMA IF NOT EXISTS analytics;";

    public const string SalesByPackageBranchMonth = @"
CREATE MATERIALIZED VIEW analytics.mv_sales_by_package_branch_month AS
SELECT coalesce(p.name, 'Sin paquete') AS package_name,
       s.is_custom_package,
       s.branch_id,
       b.name AS branch_name,
       date_trunc('month', s.scheduled_at AT TIME ZONE 'America/Costa_Rica')::date AS month,
       sum(s.total_crc) AS total_crc,
       count(*)::int AS services_count
FROM services s
JOIN branches b ON b.id = s.branch_id
LEFT JOIN packages p ON p.id = s.base_package_id
WHERE s.status IN ('Paid','InProgress','Completed')
GROUP BY 1, 2, 3, 4, 5;
CREATE INDEX ix_mv_sales_month ON analytics.mv_sales_by_package_branch_month (month);";

    public const string StageTimes = @"
CREATE MATERIALIZED VIEW analytics.mv_stage_times AS
WITH t AS (
  SELECT h.service_id, h.from_status, h.to_status, h.changed_at,
         h.changed_at - coalesce(lag(h.changed_at) OVER (PARTITION BY h.service_id ORDER BY h.changed_at), s.created_at) AS dur,
         s.branch_id
  FROM status_history h JOIN services s ON s.id = h.service_id
)
SELECT t.branch_id, b.name AS branch_name, t.from_status, t.to_status,
       date_trunc('month', t.changed_at AT TIME ZONE 'America/Costa_Rica')::date AS month,
       avg(extract(epoch FROM t.dur) / 3600.0)::numeric(10,2) AS avg_hours,
       count(*)::int AS samples
FROM t JOIN branches b ON b.id = t.branch_id
WHERE t.dur >= interval '0'
GROUP BY 1, 2, 3, 4, 5;";

    public const string ServicesByZoneMonth = @"
CREATE MATERIALIZED VIEW analytics.mv_services_by_zone_month AS
SELECT b.zone AS zone,
       date_trunc('month', s.scheduled_at AT TIME ZONE 'America/Costa_Rica')::date AS month,
       count(*)::int AS services_count
FROM services s JOIN branches b ON b.id = s.branch_id
WHERE s.status <> 'Cancelled'
GROUP BY 1, 2;";

    public const string BranchSnapshot = @"
CREATE MATERIALIZED VIEW analytics.mv_branch_snapshot AS
SELECT b.id AS branch_id, b.code, b.name, b.zone, b.lat, b.lng,
       (SELECT count(*) FROM services s WHERE s.branch_id = b.id AND s.status IN ('Registered','Invoicing','Paid','InProgress'))::int AS ongoing,
       (SELECT count(DISTINCT cb.chapel_id) FROM chapel_bookings cb JOIN chapels c ON c.id = cb.chapel_id
         WHERE c.branch_id = b.id AND cb.starts_at <= now() AND cb.ends_at > now())::int AS chapels_busy,
       (SELECT count(*) FROM chapels c WHERE c.branch_id = b.id)::int AS chapels_total,
       (SELECT count(*) FROM casket_stock k WHERE k.branch_id = b.id AND k.qty <= k.min_qty)::int AS low_stock
FROM branches b;";

    public const string DropAll = @"
DROP MATERIALIZED VIEW IF EXISTS analytics.mv_branch_snapshot;
DROP MATERIALIZED VIEW IF EXISTS analytics.mv_services_by_zone_month;
DROP MATERIALIZED VIEW IF EXISTS analytics.mv_stage_times;
DROP MATERIALIZED VIEW IF EXISTS analytics.mv_sales_by_package_branch_month;
DROP SCHEMA IF EXISTS analytics;";

    public const string RefreshAllSql = @"
REFRESH MATERIALIZED VIEW analytics.mv_sales_by_package_branch_month;
REFRESH MATERIALIZED VIEW analytics.mv_stage_times;
REFRESH MATERIALIZED VIEW analytics.mv_services_by_zone_month;
REFRESH MATERIALIZED VIEW analytics.mv_branch_snapshot;";

    public const string RefreshSnapshotSql = "REFRESH MATERIALIZED VIEW analytics.mv_branch_snapshot;";
}
