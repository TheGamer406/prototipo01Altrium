/*
 * Archivo:    backend/src/Acompana.Domain/Inventory/RestockRequest.cs
 * Qué hace:   Entidad RestockRequest, se guarda en la tabla `restock_requests`.
 * Recibe:     Campos: BranchId, CasketModelId, Qty, Status (Requested/Dispatched/Received),
 *               RequestedAt, ExternalNumber?. Los llena el handler que la crea o EF Core al leer.
 * Procesa:    Requested → Dispatched → Received.
 * Devuelve:   Nada; es estado. Hereda Id, CreatedAt y UpdatedAt de Entity.
 * Se conecta: ← DeductCasketHandler (automática), RestockRequestsHandler
 * Reglas:     RF-28
 * Estado:     Completo
 */

using Acompana.Domain.Common;
using Acompana.Domain.Enums;

namespace Acompana.Domain.Inventory;

public class RestockRequest : Entity, IHasBranch
{
    public long BranchId { get; set; }
    public long CasketModelId { get; set; }
    public int Qty { get; set; }
    public RestockStatus Status { get; set; } = RestockStatus.Requested;
    public DateTimeOffset RequestedAt { get; set; }
    public string? ExternalNumber { get; set; }

    public void Dispatch()
    {
        if (Status != RestockStatus.Requested)
            throw new DomainException("La solicitud ya fue despachada.", "restock-already-dispatched");
        Status = RestockStatus.Dispatched;
    }

    public void Receive()
    {
        if (Status != RestockStatus.Dispatched)
            throw new DomainException("La solicitud aún no ha sido despachada.", "restock-not-dispatched");
        Status = RestockStatus.Received;
    }
}
