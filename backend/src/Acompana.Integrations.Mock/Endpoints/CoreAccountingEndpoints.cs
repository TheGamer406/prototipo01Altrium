/*
 * Archivo:    backend/src/Acompana.Integrations.Mock/Endpoints/CoreAccountingEndpoints.cs
 * Qué hace:   Simula el core contable que emite la factura ante Hacienda.
 * Recibe:     POST /core-contable/invoices con Idempotency-Key y { receiver{name, nationalId},
 *               lines[], totals }.
 * Procesa:    si CoreDownUntil > now → 503
 *             si la key ya existe → misma respuesta guardada
 *             si RejectNext o cédula termina en 9 → Rejected 'Identificación del receptor inválida'
 *             si no → Accepted con coreReference FE-0000001234 (consecutivo)
 * Devuelve:   200 { status: 'Accepted'|'Rejected', coreReference?, rejectReason? } · 503.
 * Se conecta: ← CoreAccountingClient
 * Reglas:     RF-09 · RF-13
 * Estado:     Completo
 */

namespace Acompana.Integrations.Mock.Endpoints;

public record CoreReceiver(string Name, string NationalId);
public record CoreInvoiceBody(CoreReceiver Receiver, object[]? Lines, object? Totals);

public static class CoreAccountingEndpoints
{
    public static void Map(WebApplication app)
    {
        app.MapPost("/core-contable/invoices", async (HttpRequest req, CoreInvoiceBody body, MockState state) =>
        {
            await Latency.Wait();
            if (state.CoreDownUntil > DateTimeOffset.UtcNow)
                return Results.StatusCode(503);
            var key = req.Headers["Idempotency-Key"].ToString();
            if (!string.IsNullOrEmpty(key) && state.Responses.TryGetValue(key, out var saved))
                return Results.Ok(saved);

            object response;
            if (state.RejectNext || body.Receiver.NationalId.EndsWith('9'))
            {
                state.RejectNext = false;
                response = new { status = "Rejected", rejectReason = "Identificación del receptor inválida" };
            }
            else response = new { status = "Accepted", coreReference = state.NextCoreReference() };

            if (!string.IsNullOrEmpty(key)) state.Responses[key] = response;
            return Results.Ok(response);
        });
    }
}
