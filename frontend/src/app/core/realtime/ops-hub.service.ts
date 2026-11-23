/**
 * Archivo:    frontend/src/app/core/realtime/ops-hub.service.ts
 * Qué hace:   Conexión SignalR a /hubs/ops.
 * Recibe:     Sede y rol actuales (se reconecta al cambiar).
 * Procesa:    HubConnectionBuilder con reconexión automática; expone cada evento como Observable.
 * Devuelve:   serviceUpdated$, offerAwarded$, alertRaised$, cremationMoved$, stockChanged$.
 * Se conecta: → backend OpsHub · ← service-board, provider-inbox, central-view, cremation-board,
 *               topbar (alertas)
 * Reglas:     RF-19 · RF-29 · RNF-06
 * Estado:     Pendiente
 */
