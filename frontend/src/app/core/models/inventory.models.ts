/**
 * Archivo:    frontend/src/app/core/models/inventory.models.ts
 * Qué hace:   Tipos TypeScript: StockRow, RestockRequest (espejo de InventoryDtos.cs).
 * Recibe:     —
 * Procesa:    Solo interfaces y tipos unión; deben coincidir con los DTO del backend.
 * Devuelve:   —
 * Se conecta: ← core/api/inventory.api.ts y features
 * Estado:     Completo
 */

export interface StockRow { branchId: number; branchName: string; modelId: number; sku: string; modelName: string; imagePath: string; qty: number; minQty: number; level: 'ok' | 'min' | 'out'; }
export interface RestockRequest { id: number; branchName: string; sku: string; qty: number; status: string; requestedAt: string; externalNumber: string | null; }
