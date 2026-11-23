/*
 * Archivo:    backend/src/Acompana.Domain/Catalog/Component.cs
 * Qué hace:   Entidad Component, se guarda en la tabla `components`.
 * Recibe:     Campos: Name, Category, PriceCrc (decimal), TaxExemptPct (0/50/100), RequiresProvider,
 *               ProviderType?, ImagePath?, Active. Los llena el handler que la crea o EF Core al leer.
 * Procesa:    Valida TaxExemptPct ∈ {0, 50, 100} y que RequiresProvider ⇒ ProviderType no nulo.
 * Devuelve:   Nada; es estado. Hereda Id, CreatedAt y UpdatedAt de Entity.
 * Se conecta: ← PackageItem, ServiceLine · → InvoiceCalculator (exoneración por línea)
 * Reglas:     RF-06 · RF-10
 * Estado:     Pendiente
 */
