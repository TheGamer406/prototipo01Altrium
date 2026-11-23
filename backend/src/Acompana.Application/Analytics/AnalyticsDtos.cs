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
 * Estado:     Completo
 */

namespace Acompana.Application.Analytics;

public record KpiDto(int ServicesThisMonth, decimal AvgTicketCrc, decimal InsurerPaidPct, decimal AvgRegisterToPaidHours);
public record SalesByPackage(string Package, decimal TotalCrc, int Count);
public record SalesByMonth(string Month, decimal TotalCrc);
public record SalesByBranch(string Branch, decimal TotalCrc, int Count);
public record SalesDto(IReadOnlyList<SalesByPackage> ByPackage, IReadOnlyList<SalesByMonth> ByMonth, IReadOnlyList<SalesByBranch> ByBranch);
public record SalesResponse(KpiDto Kpis, SalesDto Sales);
public record StageDto(string From, string To, decimal AvgHours);
public record SlowestStageDto(string Branch, string Stage, decimal AvgHours);
public record StageTimesDto(IReadOnlyList<StageDto> Stages, IReadOnlyList<SlowestStageDto> SlowestByBranch);
public record ZoneCellDto(string Zone, string Month, int Count);
public record ZonesDto(IReadOnlyList<ZoneCellDto> Cells);
