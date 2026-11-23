/*
 * Archivo:    backend/src/Acompana.Api/Controllers/AdminController.cs
 * Qué hace:   Acciones del modo admin del demo.
 * Recibe:     Peticiones HTTP bajo /api con X-Demo-Role y X-Demo-Branch (ya resueltos en
 *               ICurrentUser).
 * Procesa:    Sin lógica de negocio: valida forma, llama al handler/query y traduce el resultado a
 *               HTTP.
 * Devuelve:   JSON camelCase; errores como ProblemDetails con mensaje en español.
 * Se conecta: → Application/Admin/*
 * Reglas:     —
 * Estado:     Completo
 *
 * Contrato:
 *   POST /api/admin/reset
 *     Recibe:     —
 *     Devuelve:   200 resumen del seed
 *   POST /api/admin/refresh-analytics
 *     Recibe:     —
 *     Devuelve:   200 { refreshedAt }
 *   POST /api/admin/simulate/concurrent-accept
 *     Recibe:     —
 *     Devuelve:   200 resultado de las dos aceptaciones
 *   POST /api/admin/simulate/next-invoice-reject
 *     Recibe:     —
 *     Devuelve:   200
 *   POST /api/admin/simulate/core-down?minutes=
 *     Recibe:     minutos (5 por defecto)
 *     Devuelve:   200
 *
 * Notas:      No requiere rol: el modo admin es del presentador.
 */

using Acompana.Application.Admin;
using Microsoft.AspNetCore.Mvc;

namespace Acompana.Api.Controllers;

[ApiController]
[Route("api/admin")]
public class AdminController : ControllerBase
{
    [HttpPost("reset")]
    public Task<object> Reset([FromServices] ResetDemoHandler h, CancellationToken ct) => h.HandleAsync(ct);

    [HttpPost("refresh-analytics")]
    public Task<object> Refresh([FromServices] RefreshAnalyticsHandler h, CancellationToken ct) => h.HandleAsync(ct);

    [HttpPost("simulate/concurrent-accept")]
    public Task<SimulationResult> ConcurrentAccept([FromServices] SimulationHandler h, CancellationToken ct) => h.ConcurrentAcceptAsync(ct);

    [HttpPost("simulate/next-invoice-reject")]
    public Task<SimulationResult> NextReject([FromServices] SimulationHandler h, CancellationToken ct) => h.NextInvoiceRejectAsync(ct);

    [HttpPost("simulate/core-down")]
    public Task<SimulationResult> CoreDown([FromServices] SimulationHandler h, int minutes = 5, CancellationToken ct = default) => h.CoreDownAsync(minutes, ct);
}
