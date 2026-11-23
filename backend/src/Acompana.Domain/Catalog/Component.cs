/*
 * Archivo:    backend/src/Acompana.Domain/Catalog/Component.cs
 * Qué hace:   Entidad Component, se guarda en la tabla `components`.
 * Recibe:     Campos: Name, Category, PriceCrc (decimal), TaxExemptPct (0/50/100), RequiresProvider,
 *               ProviderType?, ImagePath?, Active. Los llena el handler que la crea o EF Core al leer.
 * Procesa:    Valida TaxExemptPct ∈ {0, 50, 100} y que RequiresProvider ⇒ ProviderType no nulo.
 * Devuelve:   Nada; es estado. Hereda Id, CreatedAt y UpdatedAt de Entity.
 * Se conecta: ← PackageItem, ServiceLine · → InvoiceCalculator (exoneración por línea)
 * Reglas:     RF-06 · RF-10
 * Estado:     Completo
 */

using Acompana.Domain.Common;
using Acompana.Domain.Enums;

namespace Acompana.Domain.Catalog;

public class Component : Entity
{
    public string Name { get; set; } = "";
    public ComponentCategory Category { get; set; }
    public decimal PriceCrc { get; set; }
    public int TaxExemptPct { get; set; }
    public bool RequiresProvider { get; set; }
    public ProviderType? ProviderType { get; set; }
    public string? ImagePath { get; set; }
    public bool Active { get; set; } = true;

    public void Validate()
    {
        if (TaxExemptPct is not (0 or 50 or 100))
            throw new DomainException("La exoneración debe ser 0, 50 o 100 %.", "invalid-exemption");
        if (RequiresProvider && ProviderType is null)
            throw new DomainException("Indique el tipo de proveedor del componente.", "provider-type-required");
        if (PriceCrc < 0)
            throw new DomainException("El precio no puede ser negativo.", "invalid-price");
    }
}
