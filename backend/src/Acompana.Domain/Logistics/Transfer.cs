/*
 * Archivo:    backend/src/Acompana.Domain/Logistics/Transfer.cs
 * Qué hace:   Entidad Transfer, se guarda en la tabla `transfers`.
 * Recibe:     Campos: ServiceId, HearseId, DriverId, Status (TransferStatus). Los llena el handler que
 *               la crea o EF Core al leer.
 * Procesa:    LastPosition() = último hito con lat/lng (para el mapa).
 * Devuelve:   Nada; es estado. Hereda Id, CreatedAt y UpdatedAt de Entity.
 * Se conecta: ← TransfersQuery · → TransferMilestone
 * Reglas:     RF-22 · RF-23 · RF-24
 * Estado:     Pendiente
 */
