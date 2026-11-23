/*
 * Archivo:    backend/src/Acompana.Api/Controllers/OffersController.cs
 * Qué hace:   Bandeja y aceptación de ofertas del proveedor.
 * Recibe:     Peticiones HTTP bajo /api con X-Demo-Role y X-Demo-Branch (ya resueltos en
 *               ICurrentUser).
 * Procesa:    Sin lógica de negocio: valida forma, llama al handler/query y traduce el resultado a
 *               HTTP.
 * Devuelve:   JSON camelCase; errores como ProblemDetails con mensaje en español.
 * Se conecta: → Application/Providers/GetProviderInboxQuery, AcceptOfferHandler
 * Reglas:     RF-15 · RF-16 · RNF-16
 * Estado:     Completo
 *
 * Contrato:
 *   GET /api/offers?providerId=  (RF-15)
 *     Recibe:     proveedor
 *     Devuelve:   200 { open[], history[] }
 *   POST /api/offers/{id}/accept  (RF-16)
 *     Recibe:     id de la oferta (proveedor desde ICurrentUser)
 *     Devuelve:   200 AcceptOfferResult { won, message }
 */

using Acompana.Application.Common;
using Acompana.Application.Providers;
using Acompana.Domain.Enums;
using Microsoft.AspNetCore.Mvc;

namespace Acompana.Api.Controllers;

[ApiController]
[Route("api")]
public class OffersController : ControllerBase
{
    [HttpGet("offers")]
    public Task<ProviderInboxDto> Inbox([FromServices] GetProviderInboxQuery q, long? providerId, CancellationToken ct) => q.HandleAsync(providerId, ct);

    [HttpPost("offers/{id:guid}/accept")]
    public Task<AcceptOfferResult> Accept([FromServices] AcceptOfferHandler h, [FromServices] ICurrentUser user, Guid id, long? providerId, CancellationToken ct)
    {
        var pid = user.Role == DemoRole.Provider ? user.ProviderId ?? providerId : providerId;
        if (pid is null) throw new ValidationException("providerId", "Seleccione el proveedor.");
        return h.HandleAsync(id, pid.Value, ct);
    }
}
