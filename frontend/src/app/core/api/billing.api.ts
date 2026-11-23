/**
 * Archivo:    frontend/src/app/core/api/billing.api.ts
 * Qué hace:   BillingApi: cliente HTTP tipado.
 * Recibe:     Parámetros tipados con core/models.
 * Procesa:    HttpClient contra /api/services/{id}/invoice, /api/invoices/*; sin lógica de negocio.
 * Devuelve:   Observables tipados: issue(serviceId), pay(id, req), get(id).
 * Se conecta: ← features · → backend · ↔ core/models/billing.models.ts
 * Reglas:     RF-08 a RF-13
 * Estado:     Pendiente
 */
