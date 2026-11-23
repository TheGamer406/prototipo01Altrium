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
 * Estado:     Pendiente
 */
