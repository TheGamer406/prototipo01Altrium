/*
 * Archivo:    backend/src/Acompana.Domain/Enums/LogisticsEnums.cs
 * Qué hace:   Enumeraciones: MilestoneType (Departure, Pickup, ArrivalWake, DepartureWake,
 *               ArrivalDestination) · MilestoneSource (Online, Offline) · TransferStatus (Planned,
 *               OnRoute, Done) · HearseStatus (Available, InUse, Maintenance) · CremationStatus
 *               (Waiting, InOven, Packed, Delivered).
 * Recibe:     —
 * Procesa:    Se guardan como texto en PostgreSQL (HasConversion<string>) para que pgAdmin y el grafo
 *               se lean claro.
 * Devuelve:   Valores que viajan en los DTO como texto; la interfaz los traduce al español.
 * Se conecta: ← entidades, DTO, configuraciones EF · → GET /api/meta/enums
 * Reglas:     RF-21 · RF-22 · RF-23
 * Estado:     Pendiente
 */
