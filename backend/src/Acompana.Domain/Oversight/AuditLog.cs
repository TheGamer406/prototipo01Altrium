/*
 * Archivo:    backend/src/Acompana.Domain/Oversight/AuditLog.cs
 * Qué hace:   Entidad AuditLog, se guarda en la tabla `audit_log`.
 * Recibe:     Campos: At, ActorRole, ActorName, BranchId?, Entity, EntityId, Action, OldValues
 *               (jsonb), NewValues (jsonb). Los llena el handler que la crea o EF Core al leer.
 * Procesa:    Solo inserción; nunca se edita ni se borra.
 * Devuelve:   Nada; es estado. Hereda Id, CreatedAt y UpdatedAt de Entity.
 * Se conecta: ← AuditInterceptor · → AuditQuery (caso de las flores)
 * Reglas:     RF-31 · RNF-13
 * Estado:     Pendiente
 */
