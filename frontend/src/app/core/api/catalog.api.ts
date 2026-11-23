/**
 * Archivo:    frontend/src/app/core/api/catalog.api.ts
 * Qué hace:   CatalogApi: cliente HTTP tipado.
 * Recibe:     Parámetros tipados con core/models.
 * Procesa:    HttpClient contra /api/components, /api/packages, /api/casket-models; sin lógica de
 *               negocio.
 * Devuelve:   Observables tipados: components(f), packages(), casketModels(branch), save*(dto).
 * Se conecta: ← features · → backend · ↔ core/models/catalog.models.ts
 * Reglas:     RF-04 · RF-06
 * Estado:     Pendiente
 */
