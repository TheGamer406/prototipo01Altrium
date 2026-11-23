/*
 * Archivo:    backend/src/Acompana.Api/Controllers/AnalyticsController.cs
 * Qué hace:   Dashboards de gerencia.
 * Recibe:     Peticiones HTTP bajo /api con X-Demo-Role y X-Demo-Branch (ya resueltos en
 *               ICurrentUser).
 * Procesa:    Sin lógica de negocio: valida forma, llama al handler/query y traduce el resultado a
 *               HTTP.
 * Devuelve:   JSON camelCase; errores como ProblemDetails con mensaje en español.
 * Se conecta: → Application/Analytics/*
 * Reglas:     RF-32 a RF-34
 * Estado:     Completo
 *
 * Contrato:
 *   GET /api/analytics/sales?from=&to=&branch=  (RF-32)
 *     Recibe:     rango y sede
 *     Devuelve:   200 { kpis, sales }
 *   GET /api/analytics/stage-times?from=&to=  (RF-33)
 *     Recibe:     rango
 *     Devuelve:   200 StageTimesDto
 *   GET /api/analytics/zones?year=  (RF-34)
 *     Recibe:     año
 *     Devuelve:   200 ZonesDto
 */

using Acompana.Application.Analytics;
using Microsoft.AspNetCore.Mvc;

namespace Acompana.Api.Controllers;

[ApiController]
[Route("api/analytics")]
public class AnalyticsController : ControllerBase
{
    [HttpGet("sales")]
    public Task<SalesResponse> Sales([FromServices] SalesQuery q, DateOnly? from, DateOnly? to, long? branch, CancellationToken ct) => q.HandleAsync(from, to, branch, ct);

    [HttpGet("stage-times")]
    public Task<StageTimesDto> StageTimes([FromServices] StageTimesQuery q, DateOnly? from, DateOnly? to, CancellationToken ct) => q.HandleAsync(from, to, ct);

    [HttpGet("zones")]
    public Task<ZonesDto> Zones([FromServices] ZonesQuery q, int? year, CancellationToken ct) => q.HandleAsync(year, ct);
}
