/*
 * Archivo:    backend/src/Acompana.Api/Controllers/ProvidersController.cs
 * Qué hace:   Cartera de proveedores y oportunidades abiertas.
 * Recibe:     Peticiones HTTP bajo /api con X-Demo-Role y X-Demo-Branch (ya resueltos en
 *               ICurrentUser).
 * Procesa:    Sin lógica de negocio: valida forma, llama al handler/query y traduce el resultado a
 *               HTTP.
 * Devuelve:   JSON camelCase; errores como ProblemDetails con mensaje en español.
 * Se conecta: → Application/Providers/ProvidersHandler, GetOpenOpportunitiesQuery
 * Reglas:     RF-14 · RF-17
 * Estado:     Completo
 *
 * Contrato:
 *   GET /api/providers?type=&zone=&active=  (RF-14)
 *     Recibe:     filtros
 *     Devuelve:   200 ProviderDto[]
 *   POST|PUT /api/providers[/{id}]
 *     Recibe:     ProviderDto
 *     Devuelve:   200/201 ProviderDto
 *   GET /api/providers/opportunities?branch=  (RF-17)
 *     Recibe:     sede
 *     Devuelve:   200 OpenOpportunityDto[]
 */

using Acompana.Application.Providers;
using Microsoft.AspNetCore.Mvc;

namespace Acompana.Api.Controllers;

[ApiController]
[Route("api")]
public class ProvidersController : ControllerBase
{
    [HttpGet("providers")]
    public Task<IReadOnlyList<ProviderDto>> List([FromServices] ProvidersHandler h, string? type, string? zone, bool? active, CancellationToken ct) => h.ListAsync(type, zone, active, ct);

    [HttpPost("providers")]
    public async Task<IActionResult> Create([FromServices] ProvidersHandler h, ProviderDto dto, CancellationToken ct) => Created("/api/providers", await h.SaveAsync(null, dto, ct));

    [HttpPut("providers/{id:long}")]
    public Task<ProviderDto> Update([FromServices] ProvidersHandler h, long id, ProviderDto dto, CancellationToken ct) => h.SaveAsync(id, dto, ct);

    [HttpGet("providers/opportunities")]
    public Task<IReadOnlyList<OpenOpportunityDto>> Opportunities([FromServices] GetOpenOpportunitiesQuery q, long? branch, CancellationToken ct) => q.HandleAsync(branch, ct);
}
