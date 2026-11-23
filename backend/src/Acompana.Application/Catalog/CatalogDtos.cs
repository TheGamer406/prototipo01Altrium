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
 * Estado:     Completo
 */

namespace Acompana.Application.Catalog;

public record ComponentDto(long Id, string Name, string Category, decimal PriceCrc, int TaxExemptPct, bool RequiresProvider, string? ProviderType, string? ImagePath, bool Active);
public record PackageItemDto(long ComponentId, string Name, string Category, int Qty, decimal PriceCrc, int TaxExemptPct, bool RequiresProvider);
public record PackageDto(long Id, string Name, string Description, decimal PriceCrc, IReadOnlyList<PackageItemDto> Items, string? ImagePath, bool Active);
public record CasketModelDto(long Id, string Sku, string Name, string Material, string Origin, decimal PriceCrc, string ImagePath, int? StockInBranch);
