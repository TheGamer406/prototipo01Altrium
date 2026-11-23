/*
 * Archivo:    backend/src/Acompana.Domain/Providers/Provider.cs
 * Qué hace:   Entidad Provider, se guarda en la tabla `providers`.
 * Recibe:     Campos: Name, Type (ProviderType), Zone, Email, Phone, Active. Los llena el handler que
 *               la crea o EF Core al leer.
 * Procesa:    Sin lógica. 30 proveedores: 12 floristerías, 10 catering, 8 música.
 * Devuelve:   Nada; es estado. Hereda Id, CreatedAt y UpdatedAt de Entity.
 * Se conecta: ← DispatchOffersHandler (filtra por tipo y zona) · → ProviderOffer
 * Reglas:     RF-14
 * Estado:     Completo
 */

using Acompana.Domain.Common;
using Acompana.Domain.Enums;

namespace Acompana.Domain.Providers;

public class Provider : Entity
{
    public string Name { get; set; } = "";
    public ProviderType Type { get; set; }
    public Zone Zone { get; set; }
    public string Email { get; set; } = "";
    public string Phone { get; set; } = "";
    public bool Active { get; set; } = true;
}
