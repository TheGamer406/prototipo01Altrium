/*
 * Archivo:    backend/src/Acompana.Application/Analytics/AnalyticsDtos.cs
 * Qué hace:   DTO de analítica.
 * Recibe:     —
 * Procesa:    —
 * Devuelve:   KpiDto { servicesThisMonth, avgTicketCrc, insurerPaidPct, avgRegisterToPaidHours }
 *             SalesDto { byPackage[{package, totalCrc, count}], byMonth[{month, totalCrc}],
 *               byBranch[{branch, totalCrc, count}] }
 *             StageTimesDto { stages[{from, to, avgHours}], slowestByBranch[{branch, stage, avgHours}]
 *               }
 *             ZonesDto { cells[{zone, month, count}] }
 * Se conecta: ← AnalyticsController · → core/models/analytics.models.ts
 * Reglas:     RF-32 a RF-34
 * Estado:     Pendiente
 */
