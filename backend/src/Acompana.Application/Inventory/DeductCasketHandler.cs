/*
 * Archivo:    backend/src/Acompana.Application/Inventory/DeductCasketHandler.cs
 * Qué hace:   Descuenta el cofre vendido y dispara alerta + reabastecimiento si llega al mínimo.
 * Recibe:     serviceId (lo llama RegisterPaymentHandler).
 * Procesa:    busca la línea Casket; CasketStock.Deduct(1) de la sede
 *             IInventoryClient.ReportMovementAsync(-1)
 *             si Qty <= MinQty: Alert LowStock (sede y bodega) + RestockRequest automática +
 *               IInventoryClient.RequestRestockAsync
 *             notifica StockChanged
 * Devuelve:   StockRowDto de la sede/modelo.
 * Se conecta: ← RegisterPaymentHandler · → IInventoryClient, IOpsNotifier
 * Reglas:     RF-26 · RF-27 · RF-28
 * Estado:     Pendiente
 */
