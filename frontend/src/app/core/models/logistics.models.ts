/**
 * Archivo:    frontend/src/app/core/models/logistics.models.ts
 * Qué hace:   Tipos TypeScript: ChapelCalendar, Transfer, Milestone, CremationCard (espejo de
 *               LogisticsDtos.cs).
 * Recibe:     —
 * Procesa:    Solo interfaces y tipos unión; deben coincidir con los DTO del backend.
 * Devuelve:   —
 * Se conecta: ← core/api/logistics.api.ts y features
 * Estado:     Completo
 */

export interface ChapelBooking { id: string; serviceCode: string; from: string; to: string; }
export interface ChapelDay { id: number; name: string; capacity: number; bookings: ChapelBooking[]; }
export interface ChapelCalendar { chapels: ChapelDay[]; }
export interface BookChapelRequest { chapelId: number; serviceId: string; from: string; to: string; }
export interface Milestone { clientUuid: string; type: string; recordedAt: string; lat?: number | null; lng?: number | null; source: 'Online' | 'Offline'; }
export interface MilestoneBatch { items: Milestone[]; }
export interface Transfer { id: number; serviceCode: string; plate: string; driverName: string; status: string; lastLat: number | null; lastLng: number | null; milestones: Milestone[]; delayed: boolean; }
export interface CremationCard { id: number; serviceCode: string; deceasedName: string; status: string; queuePosition: number; operator: string | null; }
