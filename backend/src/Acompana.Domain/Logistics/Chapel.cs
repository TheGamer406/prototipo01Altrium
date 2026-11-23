/*
 * Archivo:    backend/src/Acompana.Domain/Logistics/Chapel.cs
 * Qué hace:   Entidad Chapel, se guarda en la tabla `chapels`.
 * Recibe:     Campos: BranchId, Name, Capacity. Los llena el handler que la crea o EF Core al leer.
 * Procesa:    Sin lógica. 2-3 por sede GAM, 0-1 fuera.
 * Devuelve:   Nada; es estado. Hereda Id, CreatedAt y UpdatedAt de Entity.
 * Se conecta: ← ChapelBooking
 * Reglas:     RF-20
 * Estado:     Pendiente
 */
