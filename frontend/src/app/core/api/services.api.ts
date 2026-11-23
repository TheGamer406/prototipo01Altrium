/**
 * Archivo:    frontend/src/app/core/api/services.api.ts
 * Qué hace:   ServicesApi: cliente HTTP tipado.
 * Recibe:     Parámetros tipados con core/models.
 * Procesa:    HttpClient contra GET/POST/PUT /api/services*, /api/prospects/export; sin lógica de
 *               negocio.
 * Devuelve:   Observables tipados: list(filters), get(id), create(req), update(id, req), activate(id,
 *               req), setPackage(id, req), uploadCertificate(id, number, file), board(id), timeline(id).
 * Se conecta: ← features · → backend · ↔ core/models/services.models.ts
 * Reglas:     RF-01 a RF-07 · RF-19
 * Estado:     Pendiente
 */
