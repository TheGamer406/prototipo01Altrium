/*
 * Archivo:    backend/src/Acompana.Api/Controllers/OversightController.cs
 * Qué hace:   Vista central, bitácora, alertas, mensajes y parámetros.
 * Recibe:     Peticiones HTTP bajo /api con X-Demo-Role y X-Demo-Branch (ya resueltos en
 *               ICurrentUser).
 * Procesa:    Sin lógica de negocio: valida forma, llama al handler/query y traduce el resultado a
 *               HTTP.
 * Devuelve:   JSON camelCase; errores como ProblemDetails con mensaje en español.
 * Se conecta: → Application/Oversight/*
 * Reglas:     RF-29 · RF-30 · RF-31
 * Estado:     Completo
 *
 * Contrato:
 *   GET /api/overview  (RF-29)
 *     Recibe:     —
 *     Devuelve:   200 OverviewDto
 *   GET /api/audit?entity=&from=&to=  (RF-31)
 *     Recibe:     filtros
 *     Devuelve:   200 PagedResult<AuditEntryDto>
 *   GET /api/alerts?branch=
 *     Recibe:     sede
 *     Devuelve:   200 AlertDto[]
 *   POST /api/alerts/{id}/ack
 *     Recibe:     id
 *     Devuelve:   204
 *   GET /api/messages?serviceId=
 *     Recibe:     servicio
 *     Devuelve:   200 PagedResult<MessageDto>
 *   GET|PUT /api/settings  (RF-17, RF-24)
 *     Recibe:     { key: value }
 *     Devuelve:   200 diccionario
 */

using Acompana.Application.Common;
using Acompana.Application.Oversight;
using Microsoft.AspNetCore.Mvc;

namespace Acompana.Api.Controllers;

[ApiController]
[Route("api")]
public class OversightController : ControllerBase
{
    [HttpGet("overview")]
    public Task<OverviewDto> Overview([FromServices] OverviewQuery q, CancellationToken ct) => q.HandleAsync(ct);

    [HttpGet("audit")]
    public Task<PagedResult<AuditEntryDto>> Audit([FromServices] AuditQuery q, string? entity, string? entityId, DateTimeOffset? from, DateTimeOffset? to,
        string? actor, Guid? serviceId, int page = 1, int pageSize = 25, CancellationToken ct = default) => q.HandleAsync(entity, entityId, from, to, actor, serviceId, page, pageSize, ct);

    [HttpGet("alerts")]
    public Task<IReadOnlyList<AlertDto>> Alerts([FromServices] AlertsHandler h, long? branch, bool all = false, CancellationToken ct = default) => h.ListAsync(branch, all, ct);

    [HttpPost("alerts/{id:long}/ack")]
    public async Task<IActionResult> Ack([FromServices] AlertsHandler h, long id, CancellationToken ct) { await h.AcknowledgeAsync(id, ct); return NoContent(); }

    [HttpGet("messages")]
    public Task<PagedResult<MessageDto>> Messages([FromServices] MessagesQuery q, Guid? serviceId, string? kind, int page = 1, int pageSize = 25, CancellationToken ct = default) => q.HandleAsync(serviceId, kind, page, pageSize, ct);

    [HttpGet("settings")]
    public Task<Dictionary<string, string>> GetSettings([FromServices] SettingsHandler h, CancellationToken ct) => h.GetAsync(ct);

    [HttpPut("settings")]
    public Task<Dictionary<string, string>> PutSettings([FromServices] SettingsHandler h, Dictionary<string, string> values, CancellationToken ct) => h.PutAsync(values, ct);
}
