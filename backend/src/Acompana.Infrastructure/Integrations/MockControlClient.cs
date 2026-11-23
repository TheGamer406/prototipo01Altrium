/*
 * Archivo:    backend/src/Acompana.Infrastructure/Integrations/MockControlClient.cs
 * Qué hace:   Implementa IMockControlClient con HttpClient tipado.
 * Recibe:     DTO de Application + base URL del servicio simulado (MockBaseUrl = http://integrations-
 *               mock:8081).
 * Procesa:    POST /_control/*; serializa JSON; reintentos según ResiliencePolicies.
 * Devuelve:   El resultado definido en IMockControlClient.
 * Se conecta: ← Application/Integrations/IMockControlClient · → Acompana.Integrations.Mock
 * Reglas:     RNF-15
 * Estado:     Completo
 */

using Acompana.Application.Common;
using Acompana.Application.Integrations;

namespace Acompana.Infrastructure.Integrations;

public sealed class MockControlClient : IMockControlClient
{
    private readonly HttpClient _http;
    public MockControlClient(HttpClient http) => _http = http;

    private async Task Post(string url, CancellationToken ct)
    {
        try { using var res = await _http.PostAsync(url, null, ct); res.EnsureSuccessStatusCode(); }
        catch (Exception ex) when (ex is HttpRequestException or TaskCanceledException)
        {
            throw new IntegrationUnavailableException("El simulador de integraciones no responde.", ex);
        }
    }

    public Task SetCoreDownAsync(int minutes, CancellationToken ct) => Post($"/_control/core-down?minutes={minutes}", ct);
    public Task RejectNextInvoiceAsync(CancellationToken ct) => Post("/_control/reject-next", ct);
    public Task ResetAsync(CancellationToken ct) => Post("/_control/reset", ct);
}
