/*
 * Archivo:    backend/src/Acompana.Infrastructure/Integrations/CoreAccountingClient.cs
 * Qué hace:   Implementa ICoreAccountingClient con HttpClient tipado.
 * Recibe:     DTO de Application + base URL del servicio simulado (MockBaseUrl = http://integrations-
 *               mock:8081).
 * Procesa:    POST /core-contable/invoices; serializa JSON; reintentos según ResiliencePolicies.
 * Devuelve:   El resultado definido en ICoreAccountingClient.
 * Se conecta: ← Application/Integrations/ICoreAccountingClient · → Acompana.Integrations.Mock
 * Reglas:     RNF-15
 * Estado:     Completo
 */

using System.Net.Http.Json;
using Acompana.Application.Common;
using Acompana.Application.Integrations;

namespace Acompana.Infrastructure.Integrations;

public sealed class CoreAccountingClient : ICoreAccountingClient
{
    private readonly HttpClient _http;
    public CoreAccountingClient(HttpClient http) => _http = http;

    private record Reply(string Status, string? CoreReference, string? RejectReason);

    public async Task<CoreInvoiceResult> SendInvoiceAsync(CoreInvoiceRequest req, string idempotencyKey, CancellationToken ct)
    {
        try
        {
            using var msg = new HttpRequestMessage(HttpMethod.Post, "/core-contable/invoices") { Content = JsonContent.Create(req) };
            msg.Headers.Add("Idempotency-Key", idempotencyKey);
            using var res = await _http.SendAsync(msg, ct);
            if (!res.IsSuccessStatusCode)
                throw new IntegrationUnavailableException("El core contable no responde; se reintentará.");
            var body = await res.Content.ReadFromJsonAsync<Reply>(new System.Text.Json.JsonSerializerOptions(System.Text.Json.JsonSerializerDefaults.Web), ct)
                       ?? throw new IntegrationUnavailableException("El core contable no responde; se reintentará.");
            return new CoreInvoiceResult(body.Status == "Accepted", body.CoreReference, body.RejectReason);
        }
        catch (Exception ex) when (ex is HttpRequestException or TaskCanceledException or Polly.Timeout.TimeoutRejectedException)
        {
            throw new IntegrationUnavailableException("El core contable no responde; se reintentará.", ex);
        }
    }
}
