/*
 * Archivo:    backend/src/Acompana.Domain/Logistics/TransferMilestone.cs
 * Qué hace:   Entidad TransferMilestone, se guarda en la tabla `transfer_milestones`.
 * Recibe:     Campos: TransferId, Type, RecordedAt, SyncedAt, Lat?, Lng?, Source (Online/Offline),
 *               ClientUuid (único). Los llena el handler que la crea o EF Core al leer.
 * Procesa:    RecordedAt = hora en el teléfono; SyncedAt = hora en que llegó al servidor. ClientUuid
 *               evita duplicados.
 * Devuelve:   Nada; es estado. Hereda Id, CreatedAt y UpdatedAt de Entity.
 * Se conecta: ← RecordMilestonesHandler · → TransferDelayChecker
 * Reglas:     RF-22 · RF-24 · RNF-05
 * Estado:     Completo
 */

using Acompana.Domain.Common;
using Acompana.Domain.Enums;

namespace Acompana.Domain.Logistics;

public class TransferMilestone : Entity
{
    public long TransferId { get; set; }
    public MilestoneType Type { get; set; }
    public DateTimeOffset RecordedAt { get; set; }
    public DateTimeOffset SyncedAt { get; set; }
    public double? Lat { get; set; }
    public double? Lng { get; set; }
    public MilestoneSource Source { get; set; }
    public Guid ClientUuid { get; set; }
}
