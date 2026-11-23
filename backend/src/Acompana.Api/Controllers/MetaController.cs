/*
 * Archivo:    backend/src/Acompana.Api/Controllers/MetaController.cs
 * Qué hace:   Datos de apoyo para selectores.
 * Recibe:     Peticiones HTTP bajo /api con X-Demo-Role y X-Demo-Branch (ya resueltos en
 *               ICurrentUser).
 * Procesa:    Sin lógica de negocio: valida forma, llama al handler/query y traduce el resultado a
 *               HTTP.
 * Devuelve:   JSON camelCase; errores como ProblemDetails con mensaje en español.
 * Se conecta: → Application/Meta/MetaQuery
 * Reglas:     RF-30
 * Estado:     Completo
 *
 * Contrato:
 *   GET /api/meta/branches
 *     Recibe:     —
 *     Devuelve:   200 sedes
 *   GET /api/meta/roles
 *     Recibe:     —
 *     Devuelve:   200 roles con etiqueta en español
 *   GET /api/meta/enums
 *     Recibe:     —
 *     Devuelve:   200 enums traducidos
 */

using Acompana.Application.Meta;
using Microsoft.AspNetCore.Mvc;

namespace Acompana.Api.Controllers;

[ApiController]
[Route("api/meta")]
public class MetaController : ControllerBase
{
    [HttpGet("branches")]
    public Task<IReadOnlyList<BranchMetaDto>> Branches([FromServices] MetaQuery q, CancellationToken ct) => q.BranchesAsync(ct);

    [HttpGet("roles")]
    public IReadOnlyList<OptionDto> Roles() => MetaQuery.Roles;

    [HttpGet("enums")]
    public IReadOnlyDictionary<string, IReadOnlyList<OptionDto>> Enums() => MetaQuery.Enums;

    [HttpGet("providers")]
    public Task<IReadOnlyList<ProviderMetaDto>> Providers([FromServices] MetaQuery q, CancellationToken ct) => q.ProvidersAsync(ct);

    [HttpGet("insurers")]
    public Task<IReadOnlyList<OptionDto>> Insurers([FromServices] MetaQuery q, CancellationToken ct) => q.InsurersAsync(ct);
}
