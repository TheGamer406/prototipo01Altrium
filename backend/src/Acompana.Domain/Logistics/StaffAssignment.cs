/*
 * Archivo:    backend/src/Acompana.Domain/Logistics/StaffAssignment.cs
 * Qué hace:   Entidad StaffAssignment, se guarda en la tabla `staff_assignments`.
 * Recibe:     Campos: ServiceId, EmployeeId, StartsAt, EndsAt. Los llena el handler que la crea o EF
 *               Core al leer.
 * Procesa:    El traslape se permite; lo detecta AssignStaffHandler y genera alerta.
 * Devuelve:   Nada; es estado. Hereda Id, CreatedAt y UpdatedAt de Entity.
 * Se conecta: ← AssignStaffHandler
 * Reglas:     RF-25
 * Estado:     Completo
 */

using Acompana.Domain.Common;

namespace Acompana.Domain.Logistics;

public class StaffAssignment : GuidEntity
{
    public Guid ServiceId { get; set; }
    public long EmployeeId { get; set; }
    public DateTimeOffset StartsAt { get; set; }
    public DateTimeOffset EndsAt { get; set; }
}
