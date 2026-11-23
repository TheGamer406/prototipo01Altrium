/*
 * Archivo:    backend/src/Acompana.Api/Controllers/LogisticsController.cs
 * Qué hace:   Capillas, personal, traslados y cremación.
 * Recibe:     Peticiones HTTP bajo /api con X-Demo-Role y X-Demo-Branch (ya resueltos en
 *               ICurrentUser).
 * Procesa:    Sin lógica de negocio: valida forma, llama al handler/query y traduce el resultado a
 *               HTTP.
 * Devuelve:   JSON camelCase; errores como ProblemDetails con mensaje en español.
 * Se conecta: → Application/Logistics/*
 * Reglas:     RF-20 a RF-25
 * Estado:     Pendiente
 *
 * Contrato:
 *   GET /api/chapels/calendar?branch=&from=&to=  (RF-20)
 *     Recibe:     sede y rango
 *     Devuelve:   200 ChapelCalendarDto
 *   POST /api/chapel-bookings  (RF-20)
 *     Recibe:     BookChapelRequest
 *     Devuelve:   201 id · 409 choque con mensaje
 *   POST /api/staff-assignments  (RF-25)
 *     Recibe:     AssignStaffRequest
 *     Devuelve:   201 AssignStaffResult (overlapWith si aplica)
 *   GET /api/transfers?branch=  (RF-23)
 *     Recibe:     sede
 *     Devuelve:   200 TransferDto[]
 *   POST /api/transfers/{id}/milestones  (RF-22)
 *     Recibe:     MilestoneBatch
 *     Devuelve:   200 { accepted, duplicates }
 *   GET /api/cremations  (RF-21)
 *     Recibe:     —
 *     Devuelve:   200 tablero por columnas
 *   POST /api/cremations/{id}/advance  (RF-21)
 *     Recibe:     { operatorId, receivedBy? }
 *     Devuelve:   200 CremationCardDto
 */
