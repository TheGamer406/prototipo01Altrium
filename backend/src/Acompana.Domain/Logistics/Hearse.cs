/*
 * Archivo:    backend/src/Acompana.Domain/Logistics/Hearse.cs
 * Qué hace:   Entidad Hearse, se guarda en la tabla `hearses`.
 * Recibe:     Campos: Plate, BranchId, Status (HearseStatus). Los llena el handler que la crea o EF
 *               Core al leer.
 * Procesa:    Sin lógica. ~35 carrozas.
 * Devuelve:   Nada; es estado. Hereda Id, CreatedAt y UpdatedAt de Entity.
 * Se conecta: ← Transfer.HearseId
 * Reglas:     RF-23
 * Estado:     Pendiente
 */
