/*
 * Archivo:    backend/src/Acompana.Application/Inventory/InventoryDtos.cs
 * Qué hace:   DTO de inventario.
 * Recibe:     —
 * Procesa:    —
 * Devuelve:   StockRowDto { branchId, branchName, modelId, sku, modelName, imagePath, qty, minQty,
 *               level ('ok'|'min'|'out') }
 *             RestockRequestDto { id, branchName, sku, qty, status, requestedAt, externalNumber? }
 * Se conecta: ← InventoryController · → core/models/inventory.models.ts
 * Reglas:     RF-26 a RF-28
 * Estado:     Completo
 */

namespace Acompana.Application.Inventory;

public record StockRowDto(long BranchId, string BranchName, long ModelId, string Sku, string ModelName, string ImagePath, int Qty, int MinQty, string Level);
public record RestockRequestDto(long Id, string BranchName, string Sku, int Qty, string Status, DateTimeOffset RequestedAt, string? ExternalNumber);
