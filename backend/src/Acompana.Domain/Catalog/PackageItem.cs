/*
 * Archivo:    backend/src/Acompana.Domain/Catalog/PackageItem.cs
 * Qué hace:   Entidad PackageItem, se guarda en la tabla `package_items`.
 * Recibe:     Campos: PackageId, ComponentId, Qty. Los llena el handler que la crea o EF Core al leer.
 * Procesa:    Qty ≥ 1.
 * Devuelve:   Nada; es estado. Hereda Id, CreatedAt y UpdatedAt de Entity.
 * Se conecta: ← Package.Items
 * Reglas:     RF-04
 * Estado:     Pendiente
 */
