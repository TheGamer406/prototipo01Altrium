/*
 * Archivo:    backend/src/Acompana.Domain/Organization/Deceased.cs
 * Qué hace:   Entidad Deceased, se guarda en la tabla `deceased`.
 * Recibe:     Campos: FullName, NationalId (1-0234-0567), BirthDate, DeathDate?, CauseNote. Los llena
 *               el handler que la crea o EF Core al leer.
 * Procesa:    DeathDate queda null en servicios anticipados hasta que se activan.
 * Devuelve:   Nada; es estado. Hereda Id, CreatedAt y UpdatedAt de Entity.
 * Se conecta: ← Service.DeceasedId
 * Reglas:     RF-01 · RF-02
 * Estado:     Pendiente
 */
