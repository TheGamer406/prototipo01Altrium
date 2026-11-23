/*
 * Archivo:    backend/src/Acompana.Application/Common/IOpsNotifier.cs
 * Qué hace:   Contrato para avisar a las pantallas en tiempo real.
 * Recibe:     Eventos: ServiceUpdated(serviceId), OfferAwarded(lineId, providerName, price, at),
 *               AlertRaised(AlertDto), CremationMoved(cremationId, status), StockChanged(branchId,
 *               modelId, qty).
 * Procesa:    —
 * Devuelve:   Task (fire and forget, no bloquea la transacción).
 * Se conecta: ← handlers y jobs · → Api/Realtime/SignalROpsNotifier (hub /hubs/ops)
 * Reglas:     RF-17 · RF-19 · RF-29 · RNF-06
 * Estado:     Pendiente
 */
