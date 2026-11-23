/*
 * Archivo:    backend/src/Acompana.Domain/Oversight/AuditLog.cs
 * Qué hace:   Entidad AuditLog, se guarda en la tabla `audit_log`.
 * Recibe:     Campos: At, ActorRole, ActorName, BranchId?, Entity, EntityId, Action, OldValues
 *               (jsonb), NewValues (jsonb). Los llena el handler que la crea o EF Core al leer.
 * Procesa:    Solo inserción; nunca se edita ni se borra.
 * Devuelve:   Nada; es estado. Hereda Id, CreatedAt y UpdatedAt de Entity.
 * Se conecta: ← AuditInterceptor · → AuditQuery (caso de las flores)
 * Reglas:     RF-31 · RNF-13
 * Estado:     Completo
 */

using Acompana.Domain.Common;
using Acompana.Domain.Enums;

namespace Acompana.Domain.Oversight;

public class AuditLog : Entity
{
    public DateTimeOffset At { get; set; }
    public string ActorRole { get; set; } = "";
    public string ActorName { get; set; } = "";
    public long? BranchId { get; set; }
    public string Entity { get; set; } = "";
    public string EntityId { get; set; } = "";
    public AuditAction Action { get; set; }
    public string OldValues { get; set; } = "{}";
    public string NewValues { get; set; } = "{}";
    /// <summary>Servicio al que pertenece el cambio (si aplica); permite la línea de tiempo.</summary>
    public Guid? ServiceId { get; set; }
}
