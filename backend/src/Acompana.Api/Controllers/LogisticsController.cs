/*
 * Archivo:    backend/src/Acompana.Api/Controllers/LogisticsController.cs
 * Qué hace:   Capillas, personal, traslados y cremación.
 * Recibe:     Peticiones HTTP bajo /api con X-Demo-Role y X-Demo-Branch (ya resueltos en
 *               ICurrentUser).
 * Procesa:    Sin lógica de negocio: valida forma, llama al handler/query y traduce el resultado a
 *               HTTP.
 * Devuelve:   JSON camelCase; errores como ProblemDetails con mensaje en español.
 * Se conecta: → Application/Logistics/*
 * Reglas:     RF-20 a RF-25
 * Estado:     Parcial: capillas, personal y cremación listos; traslados en la fase 6
 *
 * Contrato:
 *   GET /api/chapels/calendar?branch=&from=&to=  (RF-20)
 *     Recibe:     sede y rango
 *     Devuelve:   200 ChapelCalendarDto
 *   POST /api/chapel-bookings  (RF-20)
 *     Recibe:     BookChapelRequest
 *     Devuelve:   201 id · 409 choque con mensaje
 *   POST /api/staff-assignments  (RF-25)
 *     Recibe:     AssignStaffRequest
 *     Devuelve:   201 AssignStaffResult (overlapWith si aplica)
 *   GET /api/transfers?branch=  (RF-23)
 *     Recibe:     sede
 *     Devuelve:   200 TransferDto[]
 *   POST /api/transfers/{id}/milestones  (RF-22)
 *     Recibe:     MilestoneBatch
 *     Devuelve:   200 { accepted, duplicates }
 *   GET /api/cremations  (RF-21)
 *     Recibe:     —
 *     Devuelve:   200 tablero por columnas
 *   POST /api/cremations/{id}/advance  (RF-21)
 *     Recibe:     { operatorId, receivedBy? }
 *     Devuelve:   200 CremationCardDto
 */

using Acompana.Application.Logistics;
using Microsoft.AspNetCore.Mvc;

namespace Acompana.Api.Controllers;

[ApiController]
[Route("api")]
public class LogisticsController : ControllerBase
{
    [HttpGet("chapels/calendar")]
    public Task<ChapelCalendarDto> Calendar([FromServices] ChapelCalendarQuery q, long? branch, DateTimeOffset from, DateTimeOffset to, CancellationToken ct) => q.HandleAsync(branch, from, to, ct);

    [HttpPost("chapel-bookings")]
    public async Task<IActionResult> Book([FromServices] BookChapelHandler h, BookChapelRequest req, CancellationToken ct)
    {
        var id = await h.HandleAsync(req, ct);
        return Created($"/api/chapel-bookings/{id}", new { id });
    }

    [HttpGet("staff")]
    public Task<StaffBoardDto> Staff([FromServices] AssignStaffHandler h, long? branch, CancellationToken ct) => h.ListAsync(branch, ct);

    [HttpPost("staff-assignments")]
    public async Task<IActionResult> Assign([FromServices] AssignStaffHandler h, AssignStaffRequest req, CancellationToken ct)
        => Created("/api/staff", await h.HandleAsync(req, ct));

    [HttpGet("cremations")]
    public Task<CremationBoardDto> Cremations([FromServices] CremationQueueQuery q, CancellationToken ct) => q.HandleAsync(ct);

    [HttpPost("cremations/{id:long}/advance")]
    public Task<CremationCardDto> Advance([FromServices] AdvanceCremationHandler h, long id, AdvanceCremationRequest req, CancellationToken ct) => h.HandleAsync(id, req, ct);
}
