/*
 * Archivo:    backend/src/Acompana.Infrastructure/Persistence/AnalyticsViews.cs
 * Qué hace:   SQL de las 4 vistas materializadas del esquema analytics.
 * Recibe:     —
 * Procesa:    Constantes SQL que usa la migración: mv_sales_by_package_branch_month, mv_stage_times,
 *               mv_services_by_zone_month, mv_branch_snapshot; y RefreshAllSql.
 * Devuelve:   Strings SQL.
 * Se conecta: ← migración inicial, RefreshAnalyticsHandler
 * Reglas:     RF-29 · RF-32 · RF-33 · RF-34
 * Estado:     Pendiente
 */
