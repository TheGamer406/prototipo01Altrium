/*
 * Archivo:    backend/src/Acompana.Domain/Organization/Branch.cs
 * Qué hace:   Entidad Branch, se guarda en la tabla `branches`.
 * Recibe:     Campos: Code, Name, Canton, Province, Zone, Lat, Lng, HasChapels, HasCrematorium. Los
 *               llena el handler que la crea o EF Core al leer.
 * Procesa:    Sin lógica. 28 sedes: 12 GAM y 16 fuera (seed).
 * Devuelve:   Nada; es estado. Hereda Id, CreatedAt y UpdatedAt de Entity.
 * Se conecta: ← casi todo lo operativo cuelga de BranchId · → Chapels, Employees, CasketStock
 * Reglas:     RF-29 · RF-30 · RNF-09
 * Estado:     Completo
 */

using Acompana.Domain.Common;
using Acompana.Domain.Enums;

namespace Acompana.Domain.Organization;

public class Branch : Entity
{
    public string Code { get; set; } = "";
    public string Name { get; set; } = "";
    public string Canton { get; set; } = "";
    public string Province { get; set; } = "";
    public Zone Zone { get; set; }
    public double Lat { get; set; }
    public double Lng { get; set; }
    public bool HasChapels { get; set; }
    public bool HasCrematorium { get; set; }
}
