/**
 * Archivo:    frontend/src/app/core/models/catalog.models.ts
 * Qué hace:   Tipos TypeScript: Component, Package, CasketModel (espejo de CatalogDtos.cs).
 * Recibe:     —
 * Procesa:    Solo interfaces y tipos unión; deben coincidir con los DTO del backend.
 * Devuelve:   —
 * Se conecta: ← core/api/catalog.api.ts y features
 * Estado:     Completo
 */

export interface Component {
  id: number; name: string; category: string; priceCrc: number; taxExemptPct: number;
  requiresProvider: boolean; providerType: string | null; imagePath: string | null; active: boolean;
}
export interface PackageItem { componentId: number; name: string; category: string; qty: number; priceCrc: number; taxExemptPct: number; requiresProvider: boolean; }
export interface Package { id: number; name: string; description: string; priceCrc: number; items: PackageItem[]; imagePath: string | null; active: boolean; }
export interface CasketModel { id: number; sku: string; name: string; material: string; origin: string; priceCrc: number; imagePath: string; stockInBranch: number | null; }
