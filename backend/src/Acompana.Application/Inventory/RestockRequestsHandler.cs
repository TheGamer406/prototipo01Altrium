/*
 * Archivo:    backend/src/Acompana.Application/Inventory/RestockRequestsHandler.cs
 * Qué hace:   Lista y despacha solicitudes de reabastecimiento.
 * Recibe:     filtros (status?, branch?) · id para despachar.
 * Procesa:    Dispatch: Requested → Dispatched; al recibir suma al stock.
 * Devuelve:   RestockRequestDto[] · RestockRequestDto.
 * Se conecta: ← GET /api/restock-requests, POST /api/restock-requests/{id}/dispatch
 * Reglas:     RF-28
 * Estado:     Pendiente
 */
