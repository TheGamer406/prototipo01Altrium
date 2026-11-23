/*
 * Archivo:    backend/src/Acompana.Domain/Logistics/Transfer.cs
 * Qué hace:   Entidad Transfer, se guarda en la tabla `transfers`.
 * Recibe:     Campos: ServiceId, HearseId, DriverId, Status (TransferStatus). Los llena el handler que
 *               la crea o EF Core al leer.
 * Procesa:    LastPosition() = último hito con lat/lng (para el mapa).
 * Devuelve:   Nada; es estado. Hereda Id, CreatedAt y UpdatedAt de Entity.
 * Se conecta: ← TransfersQuery · → TransferMilestone
 * Reglas:     RF-22 · RF-23 · RF-24
 * Estado:     Completo
 */

using Acompana.Domain.Common;
using Acompana.Domain.Enums;

namespace Acompana.Domain.Logistics;

public class Transfer : Entity
{
    public Guid ServiceId { get; set; }
    public long HearseId { get; set; }
    public long DriverId { get; set; }
    public TransferStatus Status { get; set; }
    public List<TransferMilestone> Milestones { get; set; } = new();

    /// <summary>Último hito con coordenadas (para el mapa).</summary>
    public TransferMilestone? LastPosition() =>
        Milestones.Where(m => m.Lat != null && m.Lng != null).OrderByDescending(m => m.RecordedAt).FirstOrDefault();
}
