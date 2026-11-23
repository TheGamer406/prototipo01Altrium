/**
 * Archivo:    frontend/src/app/core/api/providers.api.ts
 * Qué hace:   ProvidersApi: cliente HTTP tipado.
 * Recibe:     Parámetros tipados con core/models.
 * Procesa:    HttpClient contra /api/providers*, /api/offers*; sin lógica de negocio.
 * Devuelve:   Observables tipados: list(f), save(dto), opportunities(branch), inbox(providerId),
 *               accept(offerId).
 * Se conecta: ← features · → backend · ↔ core/models/providers.models.ts
 * Reglas:     RF-14 a RF-17
 * Estado:     Pendiente
 */
