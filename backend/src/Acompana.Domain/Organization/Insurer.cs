/*
 * Archivo:    backend/src/Acompana.Domain/Organization/Insurer.cs
 * Qué hace:   Entidad Insurer, se guarda en la tabla `insurers`.
 * Recibe:     Campos: Name, NoticeEmail. Los llena el handler que la crea o EF Core al leer.
 * Procesa:    Sin lógica. 3 aseguradoras ficticias.
 * Devuelve:   Nada; es estado. Hereda Id, CreatedAt y UpdatedAt de Entity.
 * Se conecta: ← Service.InsurerId, Invoice (receptor)
 * Reglas:     RF-11 · RF-12
 * Estado:     Pendiente
 */
