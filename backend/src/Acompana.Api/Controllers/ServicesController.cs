/*
 * Archivo:    backend/src/Acompana.Api/Controllers/ServicesController.cs
 * Qué hace:   Endpoints del módulo Servicios (registro y venta).
 * Recibe:     Peticiones HTTP bajo /api con X-Demo-Role y X-Demo-Branch (ya resueltos en
 *               ICurrentUser).
 * Procesa:    Sin lógica de negocio: valida forma, llama al handler/query y traduce el resultado a
 *               HTTP.
 * Devuelve:   JSON camelCase; errores como ProblemDetails con mensaje en español.
 * Se conecta: → Application/Services/* (Create/Update/Activate/SetPackage/RegisterDeathCertificate,
 *               queries)
 * Reglas:     RF-01 a RF-07 · RF-19 · RF-36
 * Estado:     Completo
 *
 * Contrato:
 *   GET /api/services?status=&branch=&q=&page=  (RF-19, RF-30)
 *     Recibe:     filtros en query string
 *     Devuelve:   200 PagedResult<ServiceListItemDto>
 *   GET /api/services/{id}
 *     Recibe:     id (Guid)
 *     Devuelve:   200 ServiceDetailDto · 404
 *   POST /api/services  (RF-01, RF-02)
 *     Recibe:     CreateServiceRequest (JSON)
 *     Devuelve:   201 ServiceDetailDto + Location
 *   PUT /api/services/{id}
 *     Recibe:     UpdateServiceRequest
 *     Devuelve:   200 ServiceDetailDto · 409 concurrencia
 *   POST /api/services/{id}/activate  (RF-02)
 *     Recibe:     { deathDate, scheduledAt }
 *     Devuelve:   200 ServiceDetailDto
 *   PUT /api/services/{id}/package  (RF-04, RF-05)
 *     Recibe:     { basePackageId, lines[] }
 *     Devuelve:   200 ServiceDetailDto
 *   POST /api/services/{id}/death-certificate  (RF-03)
 *     Recibe:     multipart: number, file
 *     Devuelve:   200 DeathCertificateDto
 *   GET /api/services/{id}/board  (RF-19)
 *     Recibe:     id
 *     Devuelve:   200 ServiceBoardDto
 *   GET /api/services/{id}/timeline
 *     Recibe:     id
 *     Devuelve:   200 TimelineEntryDto[]
 *   GET /api/prospects/export?from=&to=  (RF-36)
 *     Recibe:     rango de fechas
 *     Devuelve:   200 text/csv
 */

using Acompana.Application.Billing;
using Acompana.Application.Common;
using Acompana.Application.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace Acompana.Api.Controllers;

[ApiController]
[Route("api")]
public class ServicesController : ControllerBase
{
    [HttpGet("services")]
    public Task<PagedResult<ServiceListItemDto>> List([FromServices] GetServicesQuery q, string? status, long? branch, string? term, int page = 1, int pageSize = 25, string? sort = null, CancellationToken ct = default)
        => q.HandleAsync(status, branch, term ?? Request.Query["q"].ToString(), page, pageSize, sort, ct);

    [HttpGet("services/{id:guid}")]
    public Task<ServiceDetailDto> Get([FromServices] GetServiceDetailQuery q, Guid id, CancellationToken ct) => q.HandleAsync(id, ct);

    [HttpPost("services")]
    public async Task<IActionResult> Create([FromServices] CreateServiceHandler h, CreateServiceRequest req, CancellationToken ct)
    {
        var dto = await h.HandleAsync(req, ct);
        return Created($"/api/services/{dto.Id}", dto);
    }

    [HttpPut("services/{id:guid}")]
    public Task<ServiceDetailDto> Update([FromServices] UpdateServiceHandler h, Guid id, UpdateServiceRequest req, CancellationToken ct) => h.HandleAsync(id, req, ct);

    [HttpPost("services/{id:guid}/activate")]
    public Task<ServiceDetailDto> Activate([FromServices] ActivateAdvanceServiceHandler h, Guid id, ActivateRequest req, CancellationToken ct) => h.HandleAsync(id, req, ct);

    [HttpPut("services/{id:guid}/package")]
    public Task<ServiceDetailDto> SetPackage([FromServices] SetServicePackageHandler h, Guid id, SetPackageRequest req, CancellationToken ct) => h.HandleAsync(id, req, ct);

    [HttpPost("services/{id:guid}/death-certificate")]
    [RequestSizeLimit(8 * 1024 * 1024)]
    public Task<DeathCertificateDto> DeathCertificate([FromServices] RegisterDeathCertificateHandler h, Guid id, [FromForm] string number, IFormFile file, CancellationToken ct)
        => h.HandleAsync(id, number, file, ct);

    [HttpGet("services/{id:guid}/death-certificate/file")]
    public async Task<IActionResult> DeathCertificateFile([FromServices] GetServiceDetailQuery q, [FromServices] IConfiguration config, Guid id, CancellationToken ct)
    {
        await q.HandleAsync(id, ct); // valida visibilidad por sede
        var dir = Path.Combine(RegisterDeathCertificateHandler.UploadsRoot(config), id.ToString());
        var file = Directory.Exists(dir) ? Directory.GetFiles(dir, "acta.*").FirstOrDefault() : null;
        if (file is null) throw new NotFoundException("El acta no tiene archivo.");
        var type = Path.GetExtension(file).ToLowerInvariant() switch { ".pdf" => "application/pdf", ".png" => "image/png", _ => "image/jpeg" };
        return PhysicalFile(file, type);
    }

    [HttpGet("services/{id:guid}/board")]
    public Task<ServiceBoardDto> Board([FromServices] GetServiceBoardQuery q, Guid id, CancellationToken ct) => q.HandleAsync(id, ct);

    [HttpGet("services/{id:guid}/timeline")]
    public Task<IReadOnlyList<TimelineEntryDto>> Timeline([FromServices] GetServiceTimelineQuery q, Guid id, CancellationToken ct) => q.HandleAsync(id, ct);

    [HttpGet("prospects/export")]
    public async Task<IActionResult> Export([FromServices] ExportProspectsQuery q, DateOnly from, DateOnly to, CancellationToken ct)
        => File(await q.HandleAsync(from, to, ct), "text/csv", $"prospectos-{from:yyyyMMdd}-{to:yyyyMMdd}.csv");
}
