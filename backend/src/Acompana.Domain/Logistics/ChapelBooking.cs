/*
 * Archivo:    backend/src/Acompana.Domain/Logistics/ChapelBooking.cs
 * Qué hace:   Entidad ChapelBooking, se guarda en la tabla `chapel_bookings`.
 * Recibe:     Campos: ChapelId, ServiceId, Period (NpgsqlRange<DateTimeOffset>). Los llena el handler
 *               que la crea o EF Core al leer.
 * Procesa:    El choque lo rechaza la BD: EXCLUDE USING gist (chapel_id WITH =, period WITH &&).
 * Devuelve:   Nada; es estado. Hereda Id, CreatedAt y UpdatedAt de Entity.
 * Se conecta: ← BookChapelHandler · → ChapelCalendarQuery
 * Reglas:     RF-20
 * Estado:     Pendiente
 */
