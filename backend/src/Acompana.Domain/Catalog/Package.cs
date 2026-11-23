/*
 * Archivo:    backend/src/Acompana.Domain/Catalog/Package.cs
 * Qué hace:   Entidad Package, se guarda en la tabla `packages`.
 * Recibe:     Campos: Name, Description, Active, Items (PackageItem[]). Los llena el handler que la
 *               crea o EF Core al leer.
 * Procesa:    Precio = suma de componentes × qty (propiedad calculada, no columna). El paquete base
 *               nunca se modifica desde un servicio.
 * Devuelve:   Nada; es estado. Hereda Id, CreatedAt y UpdatedAt de Entity.
 * Se conecta: ← Service.BasePackageId · → SetServicePackageHandler copia sus ítems a ServiceLine
 * Reglas:     RF-04 · RF-05 · RF-06
 * Estado:     Completo
 */

using Acompana.Domain.Common;

namespace Acompana.Domain.Catalog;

public class Package : Entity
{
    public string Name { get; set; } = "";
    public string Description { get; set; } = "";
    public string? ImagePath { get; set; }
    public bool Active { get; set; } = true;
    public List<PackageItem> Items { get; set; } = new();

    /// <summary>Precio = Σ componente × qty (requiere cargar Items.Component).</summary>
    public decimal PriceCrc => Items.Sum(i => (i.Component?.PriceCrc ?? 0m) * i.Qty);
}
