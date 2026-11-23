/*
 * Archivo:    backend/src/Acompana.Api/Controllers/BillingController.cs
 * Qué hace:   Endpoints de facturación y cobro.
 * Recibe:     Peticiones HTTP bajo /api con X-Demo-Role y X-Demo-Branch (ya resueltos en
 *               ICurrentUser).
 * Procesa:    Sin lógica de negocio: valida forma, llama al handler/query y traduce el resultado a
 *               HTTP.
 * Devuelve:   JSON camelCase; errores como ProblemDetails con mensaje en español.
 * Se conecta: → Application/Billing/*
 * Reglas:     RF-08 a RF-13
 * Estado:     Completo
 *
 * Contrato:
 *   POST /api/services/{id}/invoice  (RF-09, RF-10, RF-11, RF-13)
 *     Recibe:     id del servicio
 *     Devuelve:   200 InvoiceDto (Accepted o Rejected) · 422 sin acta · 503 core caído
 *   POST /api/invoices/{id}/pay  (RF-08)
 *     Recibe:     PayInvoiceRequest
 *     Devuelve:   200 InvoiceDto
 *   GET /api/invoices/{id}
 *     Recibe:     id
 *     Devuelve:   200 InvoiceDto
 */

using Acompana.Application.Billing;
using Microsoft.AspNetCore.Mvc;

namespace Acompana.Api.Controllers;

[ApiController]
[Route("api")]
public class BillingController : ControllerBase
{
    [HttpPost("services/{id:guid}/invoice")]
    public Task<InvoiceDto> Issue([FromServices] IssueInvoiceHandler h, Guid id, CancellationToken ct) => h.HandleAsync(id, ct);

    [HttpPost("invoices/{id:guid}/pay")]
    public Task<InvoiceDto> Pay([FromServices] RegisterPaymentHandler h, Guid id, PayInvoiceRequest req, CancellationToken ct) => h.HandleAsync(id, req, ct);

    [HttpGet("invoices/{id:guid}")]
    public Task<InvoiceDto> Get([FromServices] GetInvoiceQuery q, Guid id, CancellationToken ct) => q.HandleAsync(id, ct);
}
