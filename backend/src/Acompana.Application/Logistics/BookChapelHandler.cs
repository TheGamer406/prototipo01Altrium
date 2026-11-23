/*
 * Archivo:    backend/src/Acompana.Application/Logistics/BookChapelHandler.cs
 * Qué hace:   Reserva una capilla sin traslapes.
 * Recibe:     BookChapelRequest.
 * Procesa:    Inserta ChapelBooking; si PostgreSQL lanza 23P01 (exclusion_violation) busca la reserva
 *               que choca y arma el mensaje.
 * Devuelve:   Id de la reserva.
 * Se conecta: ← POST /api/chapel-bookings, CreateServiceHandler · probado en ChapelOverlapTests
 * Reglas:     RF-20
 * Estado:     Pendiente
 *
 * Contrato:
 *   Task<Guid> HandleAsync(BookChapelRequest req, CancellationToken ct)
 *     Recibe:     capilla, servicio, rango
 *     Procesa:    insert + captura de exclusión
 *     Devuelve:   id
 *     Errores:    409 'La capilla {nombre} está reservada para {código} de {hh:mm} a {hh:mm}.'
 */
