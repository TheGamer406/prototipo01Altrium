/*
 * Archivo:    backend/src/Acompana.Domain/Logistics/ChapelBooking.cs
 * Qué hace:   Entidad ChapelBooking, se guarda en la tabla `chapel_bookings`.
 * Recibe:     Campos: ChapelId, ServiceId, StartsAt, EndsAt. Los llena el handler
 *               que la crea o EF Core al leer.
 * Procesa:    El choque lo rechaza la BD: EXCLUDE USING gist (chapel_id WITH =, tstzrange(starts_at, ends_at) WITH &&).
 * Devuelve:   Nada; es estado. Hereda Id, CreatedAt y UpdatedAt de Entity.
 * Se conecta: ← BookChapelHandler · → ChapelCalendarQuery
 * Reglas:     RF-20
 * Estado:     Completo
 */

using Acompana.Domain.Common;

namespace Acompana.Domain.Logistics;

public class ChapelBooking : GuidEntity
{
    public long ChapelId { get; set; }
    public Chapel? Chapel { get; set; }
    public Guid ServiceId { get; set; }
    public DateTimeOffset StartsAt { get; set; }
    public DateTimeOffset EndsAt { get; set; }
}
