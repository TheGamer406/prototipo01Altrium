/*
 * Archivo:    backend/src/Acompana.Api/Hubs/OpsHub.cs
 * Qué hace:   Hub SignalR /hubs/ops para actualizar pantallas en vivo.
 * Recibe:     Conexiones del frontend con ?branch= (grupo por sede) y grupo 'management' para
 *               gerencia.
 * Procesa:    OnConnectedAsync agrega al grupo; no recibe mensajes del cliente (solo servidor →
 *               cliente).
 * Devuelve:   Eventos: ServiceUpdated, OfferAwarded, AlertRaised, CremationMoved, StockChanged.
 * Se conecta: ← SignalROpsNotifier · → frontend core/realtime/ops-hub.service.ts
 * Reglas:     RF-19 · RF-29 · RNF-06
 * Estado:     Pendiente
 */
