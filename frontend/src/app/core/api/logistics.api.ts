/**
 * Archivo:    frontend/src/app/core/api/logistics.api.ts
 * Qué hace:   LogisticsApi: cliente HTTP tipado.
 * Recibe:     Parámetros tipados con core/models.
 * Procesa:    HttpClient contra /api/chapels, /api/chapel-bookings, /api/staff-assignments,
 *               /api/transfers, /api/cremations; sin lógica de negocio.
 * Devuelve:   Observables tipados: chapelCalendar(b, from, to), book(req), assignStaff(req),
 *               transfers(b), sendMilestones(id, batch), cremations(), advanceCremation(id, req).
 * Se conecta: ← features · → backend · ↔ core/models/logistics.models.ts
 * Reglas:     RF-20 a RF-25
 * Estado:     Pendiente
 */
