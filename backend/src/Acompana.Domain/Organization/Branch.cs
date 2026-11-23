/*
 * Archivo:    backend/src/Acompana.Domain/Organization/Branch.cs
 * Qué hace:   Entidad Branch, se guarda en la tabla `branches`.
 * Recibe:     Campos: Code, Name, Canton, Province, Zone, Lat, Lng, HasChapels, HasCrematorium. Los
 *               llena el handler que la crea o EF Core al leer.
 * Procesa:    Sin lógica. 28 sedes: 12 GAM y 16 fuera (seed).
 * Devuelve:   Nada; es estado. Hereda Id, CreatedAt y UpdatedAt de Entity.
 * Se conecta: ← casi todo lo operativo cuelga de BranchId · → Chapels, Employees, CasketStock
 * Reglas:     RF-29 · RF-30 · RNF-09
 * Estado:     Pendiente
 */
