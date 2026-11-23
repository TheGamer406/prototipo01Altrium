/**
 * Archivo:    frontend/src/app/core/models/oversight.models.ts
 * Qué hace:   Tipos TypeScript: Overview, AuditEntry, Alert, Message (espejo de OversightDtos.cs).
 * Recibe:     —
 * Procesa:    Solo interfaces y tipos unión; deben coincidir con los DTO del backend.
 * Devuelve:   —
 * Se conecta: ← core/api/oversight.api.ts y features
 * Estado:     Completo
 */

export interface BranchOverview { id: number; code: string; name: string; zone: string; lat: number; lng: number; ongoing: number; chapelsBusy: number; chapelsTotal: number; lowStock: number; }
export interface Overview { kpis: { ongoing: number; chapelsBusy: number; lowStock: number; openAlerts: number }; branches: BranchOverview[]; }
export interface AuditEntry { id: number; at: string; actorRole: string; actorName: string; branchName: string | null; entity: string; entityId: string; action: 'Create' | 'Update' | 'Delete'; oldValues: string; newValues: string; }
export interface Alert { id: number; type: string; branchName: string; serviceCode: string | null; message: string; createdAt: string; acknowledgedAt: string | null; }
export interface Message { id: number; toAddress: string; kind: string; subject: string; body: string; serviceCode: string | null; createdAt: string; }
