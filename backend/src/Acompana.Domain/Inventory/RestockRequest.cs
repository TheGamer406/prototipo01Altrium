/*
 * Archivo:    backend/src/Acompana.Domain/Inventory/RestockRequest.cs
 * Qué hace:   Entidad RestockRequest, se guarda en la tabla `restock_requests`.
 * Recibe:     Campos: BranchId, CasketModelId, Qty, Status (Requested/Dispatched/Received),
 *               RequestedAt, ExternalNumber?. Los llena el handler que la crea o EF Core al leer.
 * Procesa:    Requested → Dispatched → Received.
 * Devuelve:   Nada; es estado. Hereda Id, CreatedAt y UpdatedAt de Entity.
 * Se conecta: ← DeductCasketHandler (automática), RestockRequestsHandler
 * Reglas:     RF-28
 * Estado:     Pendiente
 */
