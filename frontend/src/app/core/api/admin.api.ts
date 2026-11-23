/**
 * Archivo:    frontend/src/app/core/api/admin.api.ts
 * Qué hace:   AdminApi: cliente HTTP tipado.
 * Recibe:     Parámetros tipados con core/models.
 * Procesa:    HttpClient contra /api/admin/*; sin lógica de negocio.
 * Devuelve:   Observables tipados: reset(), refreshAnalytics(), simulateConcurrentAccept(),
 *               rejectNextInvoice(), coreDown(minutes).
 * Se conecta: ← features · → backend · ↔ core/models/admin.models.ts
 * Reglas:     —
 * Estado:     Pendiente
 */
