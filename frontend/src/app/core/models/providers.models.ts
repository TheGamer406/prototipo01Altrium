/**
 * Archivo:    frontend/src/app/core/models/providers.models.ts
 * Qué hace:   Tipos TypeScript: Provider, Offer, OpenOpportunity, AcceptOfferResult (espejo de
 *               ProviderDtos.cs).
 * Recibe:     —
 * Procesa:    Solo interfaces y tipos unión; deben coincidir con los DTO del backend.
 * Devuelve:   —
 * Se conecta: ← core/api/providers.api.ts y features
 * Estado:     Completo
 */

export interface Provider { id: number; name: string; type: string; zone: string; email: string; phone: string; active: boolean; wonLast30d: number; }
export interface Offer {
  id: string; serviceCode: string; componentName: string; category: string; place: string; scheduledAt: string;
  fixedPriceCrc: number; status: 'Open' | 'Won' | 'Lost' | 'Expired'; sentAt: string; expiresAt: string; imagePath: string | null;
}
export interface ProviderInbox { open: Offer[]; history: Offer[]; }
export interface OpenOpportunity { lineId: string; serviceCode: string; branchName: string; componentName: string; offersSent: number; minutesOpen: number; isOverdue: boolean; }
export interface AcceptOfferResult { won: boolean; message: string; }
