/**
 * Archivo:    frontend/src/app/core/models/services.models.ts
 * Qué hace:   Tipos TypeScript: DemoRole, ServiceStatus, ServiceListItem, ServiceDetail, ServiceLine,
 *               ServiceBoard, CreateServiceRequest, TimelineEntry (espejo de ServiceDtos.cs).
 * Recibe:     —
 * Procesa:    Solo interfaces y tipos unión; deben coincidir con los DTO del backend.
 * Devuelve:   —
 * Se conecta: ← core/api/services.api.ts y features
 * Estado:     Completo
 */

import { Invoice } from './billing.models';

export type DemoRole = 'Advisor' | 'BranchManager' | 'CrematoriumOperator' | 'Provider' | 'Management' | 'Driver';
export type ServiceStatus = 'Advance' | 'Registered' | 'Invoicing' | 'Paid' | 'InProgress' | 'Completed' | 'Cancelled';
export type LineStatus = 'Pending' | 'Offered' | 'Accepted' | 'Delivered';
export type ServiceKind = 'Immediate' | 'Advance';
export type WakePlace = 'Chapel' | 'Church' | 'Home';
export type FinalDestination = 'Cemetery' | 'Cremation';
export type Payer = 'Family' | 'Insurer';

export interface ServiceListItem {
  id: string; code: string; deceasedName: string; branchName: string; status: ServiceStatus; kind: ServiceKind;
  scheduledAt: string; totalCrc: number; isCustomPackage: boolean; hasUrgentLine: boolean;
}
export interface Deceased { fullName: string; nationalId: string; birthDate: string; deathDate: string | null; causeNote: string; }
export interface Family { fullName: string; nationalId: string; relationship: string; phone: string; email: string; insuranceProspectConsent: boolean; }
export interface ServiceLine {
  id: string; componentId: number; componentName: string; category: string; imagePath: string | null; qty: number;
  unitPriceCrc: number; taxExemptPct: number; lineTotalCrc: number; status: LineStatus; providerName: string | null;
  acceptedPriceCrc: number | null; acceptedAt: string | null; dueAt: string | null; isUrgent: boolean;
  casketModelId: number | null; casketModelName: string | null;
}
export interface DeathCertificate { number: string; fileUrl: string; registeredBy: string; registeredAt: string; }
export interface ServiceDetail extends ServiceListItem {
  branchId: number; deceased: Deceased; family: Family; basePackageId: number | null; basePackage: string | null;
  lines: ServiceLine[]; deathCertificate: DeathCertificate | null; invoice: Invoice | null;
  wake: { place: WakePlace; address: string | null; chapelId: number | null; chapelName: string | null; from: string | null; to: string | null };
  destination: { kind: FinalDestination; cemeteryName: string | null };
  payer: { payer: Payer; insurerId: number | null; insurerName: string | null; policyNumber: string | null };
  version: number;
}
export interface ServiceBoard { serviceId: string; code: string; lines: ServiceLine[]; }
export interface TimelineEntry { at: string; kind: 'status' | 'audit' | 'message'; text: string; }
export interface LineInput { componentId: number; casketModelId: number | null; qty: number; }
export interface CreateServiceRequest {
  kind: ServiceKind; branchId: number;
  deceased: { fullName: string; nationalId: string; birthDate: string; deathDate: string | null };
  family: Family; basePackageId: number; lines: LineInput[]; wakePlace: WakePlace; wakeAddress: string | null; chapelId: number | null;
  finalDestination: FinalDestination; cemeteryName: string | null; scheduledAt: string; payer: Payer; insurerId: number | null; policyNumber: string | null;
}
export interface ServiceFilters { status?: string; branch?: number | null; q?: string; page?: number; pageSize?: number; sort?: string; }
