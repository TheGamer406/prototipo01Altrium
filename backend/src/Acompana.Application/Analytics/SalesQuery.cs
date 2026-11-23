/*
 * Archivo:    backend/src/Acompana.Application/Analytics/SalesQuery.cs
 * Qué hace:   Ventas por paquete, sede y mes + KPI.
 * Recibe:     from, to, branch?
 * Procesa:    Lee analytics.mv_sales_by_package_branch_month (almacén analítico separado de la
 *               operación).
 * Devuelve:   DTO de la sección correspondiente.
 * Se conecta: ← GET /api/analytics/... · → analytics.mv_sales_by_package_branch_month
 * Reglas:     RF-32
 * Estado:     Pendiente
 */
