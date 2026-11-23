/*
 * Archivo:    backend/src/Acompana.Domain/Services/StatusHistory.cs
 * Qué hace:   Entidad StatusHistory, se guarda en la tabla `status_history`.
 * Recibe:     Campos: ServiceId, FromStatus, ToStatus, ChangedBy, ChangedAt. Los llena el handler que
 *               la crea o EF Core al leer.
 * Procesa:    Solo inserción. Es la base de los tiempos por etapa de la analítica.
 * Devuelve:   Nada; es estado. Hereda Id, CreatedAt y UpdatedAt de Entity.
 * Se conecta: ← Service.ChangeStatus · → analytics.mv_stage_times, GetServiceTimelineQuery
 * Reglas:     RF-19 · RF-33
 * Estado:     Completo
 */

using Acompana.Domain.Common;
using Acompana.Domain.Enums;

namespace Acompana.Domain.Services;

public class StatusHistory : Entity
{
    public Guid ServiceId { get; set; }
    public ServiceStatus FromStatus { get; set; }
    public ServiceStatus ToStatus { get; set; }
    public string ChangedBy { get; set; } = "";
    public DateTimeOffset ChangedAt { get; set; }
}
