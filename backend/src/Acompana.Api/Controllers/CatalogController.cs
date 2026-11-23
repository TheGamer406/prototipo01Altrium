/*
 * Archivo:    backend/src/Acompana.Api/Controllers/CatalogController.cs
 * Qué hace:   Endpoints del catálogo: componentes, paquetes y modelos de cofre.
 * Recibe:     Peticiones HTTP bajo /api con X-Demo-Role y X-Demo-Branch (ya resueltos en
 *               ICurrentUser).
 * Procesa:    Sin lógica de negocio: valida forma, llama al handler/query y traduce el resultado a
 *               HTTP.
 * Devuelve:   JSON camelCase; errores como ProblemDetails con mensaje en español.
 * Se conecta: → Application/Catalog/*
 * Reglas:     RF-04 · RF-06
 * Estado:     Completo
 *
 * Contrato:
 *   GET /api/components?category=&active=
 *     Recibe:     filtros
 *     Devuelve:   200 ComponentDto[]
 *   POST|PUT /api/components[/{id}]
 *     Recibe:     ComponentDto
 *     Devuelve:   200/201 ComponentDto · 403 si no es Gerencia
 *   GET /api/packages
 *     Recibe:     —
 *     Devuelve:   200 PackageDto[]
 *   POST|PUT /api/packages[/{id}]
 *     Recibe:     PackageDto
 *     Devuelve:   200/201 PackageDto
 *   GET /api/casket-models?branch=
 *     Recibe:     sede para incluir stock
 *     Devuelve:   200 CasketModelDto[]
 *   POST|PUT /api/casket-models[/{id}]
 *     Recibe:     CasketModelDto
 *     Devuelve:   200/201 CasketModelDto
 */

using Acompana.Application.Catalog;
using Acompana.Application.Common;
using Microsoft.AspNetCore.Mvc;

namespace Acompana.Api.Controllers;

[ApiController]
[Route("api")]
public class CatalogController : ControllerBase
{
    [HttpGet("components")]
    public Task<IReadOnlyList<ComponentDto>> Components([FromServices] ComponentsHandler h, string? category, bool? active, CancellationToken ct) => h.ListAsync(category, active, ct);

    [HttpGet("packages")]
    public Task<IReadOnlyList<PackageDto>> Packages([FromServices] PackagesHandler h, bool? active, CancellationToken ct) => h.ListAsync(active, ct);

    [HttpGet("casket-models")]
    public Task<IReadOnlyList<CasketModelDto>> CasketModels([FromServices] CasketModelsHandler h, long? branch, CancellationToken ct) => h.ListAsync(branch, ct);

    [HttpPost("components")]
    public async Task<IActionResult> CreateComponent([FromServices] ComponentsHandler h, [FromServices] ICurrentUser u, ComponentDto dto, CancellationToken ct) => Created("/api/components", await h.SaveAsync(null, dto, u, ct));
    [HttpPut("components/{id:long}")]
    public Task<ComponentDto> UpdateComponent([FromServices] ComponentsHandler h, [FromServices] ICurrentUser u, long id, ComponentDto dto, CancellationToken ct) => h.SaveAsync(id, dto, u, ct);
    [HttpPost("packages")]
    public async Task<IActionResult> CreatePackage([FromServices] PackagesHandler h, [FromServices] ICurrentUser u, PackageDto dto, CancellationToken ct) => Created("/api/packages", await h.SaveAsync(null, dto, u, ct));
    [HttpPut("packages/{id:long}")]
    public Task<PackageDto> UpdatePackage([FromServices] PackagesHandler h, [FromServices] ICurrentUser u, long id, PackageDto dto, CancellationToken ct) => h.SaveAsync(id, dto, u, ct);
    [HttpPost("casket-models")]
    public async Task<IActionResult> CreateCasket([FromServices] CasketModelsHandler h, CasketModelDto dto, CancellationToken ct) => Created("/api/casket-models", await h.SaveAsync(null, dto, ct));
    [HttpPut("casket-models/{id:long}")]
    public Task<CasketModelDto> UpdateCasket([FromServices] CasketModelsHandler h, long id, CasketModelDto dto, CancellationToken ct) => h.SaveAsync(id, dto, ct);
}
