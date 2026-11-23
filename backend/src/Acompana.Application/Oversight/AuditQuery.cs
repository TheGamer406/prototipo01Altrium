/*
 * Archivo:    backend/src/Acompana.Application/Oversight/AuditQuery.cs
 * Qué hace:   Bitácora filtrable.
 * Recibe:     entity?, entityId?, from?, to?, actor?, page.
 * Procesa:    Filtra audit_log; solo Management.
 * Devuelve:   PagedResult<AuditEntryDto>.
 * Se conecta: ← GET /api/audit · pestaña Bitácora del servicio
 * Reglas:     RF-31
 * Estado:     Pendiente
 */
