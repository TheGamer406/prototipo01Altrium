/**
 * Archivo:    frontend/src/app/core/api/oversight.api.ts
 * Qué hace:   OversightApi: cliente HTTP tipado.
 * Recibe:     Parámetros tipados con core/models.
 * Procesa:    HttpClient contra /api/overview, /api/audit, /api/alerts, /api/messages, /api/settings;
 *               sin lógica de negocio.
 * Devuelve:   Observables tipados: overview(), audit(f), alerts(b), ack(id), messages(f), settings(),
 *               saveSettings(map).
 * Se conecta: ← features · → backend · ↔ core/models/oversight.models.ts
 * Reglas:     RF-29 · RF-31
 * Estado:     Pendiente
 */
