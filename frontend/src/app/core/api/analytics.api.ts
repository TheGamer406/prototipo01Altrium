/**
 * Archivo:    frontend/src/app/core/api/analytics.api.ts
 * Qué hace:   AnalyticsApi: cliente HTTP tipado.
 * Recibe:     Parámetros tipados con core/models.
 * Procesa:    HttpClient contra /api/analytics/*; sin lógica de negocio.
 * Devuelve:   Observables tipados: sales(from, to, b), stageTimes(from, to), zones(year).
 * Se conecta: ← features · → backend · ↔ core/models/analytics.models.ts
 * Reglas:     RF-32 a RF-34
 * Estado:     Pendiente
 */
