/**
 * Archivo:    frontend/src/app/core/api/inventory.api.ts
 * Qué hace:   InventoryApi: cliente HTTP tipado.
 * Recibe:     Parámetros tipados con core/models.
 * Procesa:    HttpClient contra /api/stock, /api/restock-requests; sin lógica de negocio.
 * Devuelve:   Observables tipados: stock(b), restockRequests(f), dispatch(id).
 * Se conecta: ← features · → backend · ↔ core/models/inventory.models.ts
 * Reglas:     RF-26 a RF-28
 * Estado:     Pendiente
 */
