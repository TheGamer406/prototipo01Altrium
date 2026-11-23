/**
 * Archivo:    frontend/src/app/core/models/analytics.models.ts
 * Qué hace:   Tipos TypeScript: Kpi, Sales, StageTimes, Zones (espejo de AnalyticsDtos.cs).
 * Recibe:     —
 * Procesa:    Solo interfaces y tipos unión; deben coincidir con los DTO del backend.
 * Devuelve:   —
 * Se conecta: ← core/api/analytics.api.ts y features
 * Estado:     Completo
 */

export interface Kpi { servicesThisMonth: number; avgTicketCrc: number; insurerPaidPct: number; avgRegisterToPaidHours: number; }
export interface SalesByPackage { package: string; totalCrc: number; count: number; }
export interface SalesByMonth { month: string; totalCrc: number; }
export interface SalesByBranch { branch: string; totalCrc: number; count: number; }
export interface Sales { byPackage: SalesByPackage[]; byMonth: SalesByMonth[]; byBranch: SalesByBranch[]; }
export interface SalesResponse { kpis: Kpi; sales: Sales; }
export interface Stage { from: string; to: string; avgHours: number; }
export interface SlowestStage { branch: string; stage: string; avgHours: number; }
export interface StageTimes { stages: Stage[]; slowestByBranch: SlowestStage[]; }
export interface ZoneCell { zone: string; month: string; count: number; }
export interface Zones { cells: ZoneCell[]; }
