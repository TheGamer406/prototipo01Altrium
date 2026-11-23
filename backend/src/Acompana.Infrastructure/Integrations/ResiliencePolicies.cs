/*
 * Archivo:    backend/src/Acompana.Infrastructure/Integrations/ResiliencePolicies.cs
 * Qué hace:   Políticas Polly compartidas por los clientes HTTP.
 * Recibe:     —
 * Procesa:    3 reintentos con espera exponencial (0,5 s, 1 s, 2 s) ante 5xx, 408 y errores de red;
 *               timeout 5 s por intento.
 * Devuelve:   IAsyncPolicy<HttpResponseMessage>.
 * Se conecta: ← DependencyInjection (AddPolicyHandler)
 * Reglas:     RNF-15
 * Estado:     Completo
 */

using Polly;
using Polly.Extensions.Http;

namespace Acompana.Infrastructure.Integrations;

public static class ResiliencePolicies
{
    public static IAsyncPolicy<HttpResponseMessage> Retry() =>
        HttpPolicyExtensions.HandleTransientHttpError()
            .WaitAndRetryAsync(new[] { TimeSpan.FromMilliseconds(500), TimeSpan.FromSeconds(1), TimeSpan.FromSeconds(2) });

    public static IAsyncPolicy<HttpResponseMessage> Timeout() =>
        Policy.TimeoutAsync<HttpResponseMessage>(TimeSpan.FromSeconds(5));
}
