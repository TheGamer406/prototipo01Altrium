/*
 * Archivo:    backend/src/Acompana.Integrations.Mock/Endpoints/InventoryEndpoints.cs
 * Qué hace:   Simula el sistema de inventario actual y la bodega central.
 * Recibe:     POST /inventario/movements { branchId, sku, delta } · POST /inventario/restock {
 *               branchId, sku, qty }.
 * Procesa:    Lleva un saldo ficticio por sede/SKU; numera solicitudes BOD-000001.
 * Devuelve:   200 { balance } · 201 { requestNumber }.
 * Se conecta: ← InventoryClient
 * Reglas:     RF-26 · RF-28
 * Estado:     Pendiente
 */
