/*
 * Archivo:    backend/src/Acompana.Application/Catalog/CatalogDtos.cs
 * Qué hace:   DTO del catálogo.
 * Recibe:     —
 * Procesa:    —
 * Devuelve:   ComponentDto { id, name, category, priceCrc, taxExemptPct, requiresProvider,
 *               providerType?, imagePath?, active }
 *             PackageDto { id, name, description, priceCrc (calculado), items[{componentId, name, qty,
 *               priceCrc}], imagePath? (velorioMuestraN) }
 *             CasketModelDto { id, sku, name, material, origin, priceCrc, imagePath, stockInBranch? }
 * Se conecta: ← CatalogController · → core/models/catalog.models.ts
 * Reglas:     RF-04 · RF-06
 * Estado:     Pendiente
 */
