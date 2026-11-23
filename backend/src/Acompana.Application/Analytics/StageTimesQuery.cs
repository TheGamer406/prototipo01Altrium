/*
 * Archivo:    backend/src/Acompana.Application/Analytics/StageTimesQuery.cs
 * Qué hace:   Tiempo promedio entre estados y etapa más lenta por sede.
 * Recibe:     from, to
 * Procesa:    Lee analytics.mv_stage_times (almacén analítico separado de la operación).
 * Devuelve:   DTO de la sección correspondiente.
 * Se conecta: ← GET /api/analytics/... · → analytics.mv_stage_times
 * Reglas:     RF-33
 * Estado:     Pendiente
 */
