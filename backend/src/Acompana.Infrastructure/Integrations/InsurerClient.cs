/*
 * Archivo:    backend/src/Acompana.Infrastructure/Integrations/InsurerClient.cs
 * Qué hace:   Implementa IInsurerClient con HttpClient tipado.
 * Recibe:     DTO de Application + base URL del servicio simulado (MockBaseUrl = http://integrations-
 *               mock:8081).
 * Procesa:    POST /aseguradoras/notices; serializa JSON; reintentos según ResiliencePolicies.
 * Devuelve:   El resultado definido en IInsurerClient.
 * Se conecta: ← Application/Integrations/IInsurerClient · → Acompana.Integrations.Mock
 * Reglas:     RNF-15
 * Estado:     Completo
 */

using System.Net.Http.Json;
using Acompana.Application.Common;
using Acompana.Application.Integrations;

namespace Acompana.Infrastructure.Integrations;

public sealed class InsurerClient : IInsurerClient
{
    private readonly HttpClient _http;
    public InsurerClient(HttpClient http) => _http = http;

    public async Task SendNoticeAsync(InsurerNotice notice, CancellationToken ct)
    {
        try
        {
            using var res = await _http.PostAsJsonAsync("/aseguradoras/notices", notice, ct);
            res.EnsureSuccessStatusCode();
        }
        catch (Exception ex) when (ex is HttpRequestException or TaskCanceledException or Polly.Timeout.TimeoutRejectedException)
        {
            throw new IntegrationUnavailableException("La aseguradora no responde.", ex);
        }
    }
}
