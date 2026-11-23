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
 * Estado:     Pendiente
 */
