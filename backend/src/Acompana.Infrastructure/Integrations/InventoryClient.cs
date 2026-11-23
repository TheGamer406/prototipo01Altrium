/*
 * Archivo:    backend/src/Acompana.Infrastructure/Integrations/InventoryClient.cs
 * Qué hace:   Implementa IInventoryClient con HttpClient tipado.
 * Recibe:     DTO de Application + base URL del servicio simulado (MockBaseUrl = http://integrations-
 *               mock:8081).
 * Procesa:    POST /inventario/movements, /inventario/restock; serializa JSON; reintentos según
 *               ResiliencePolicies.
 * Devuelve:   El resultado definido en IInventoryClient.
 * Se conecta: ← Application/Integrations/IInventoryClient · → Acompana.Integrations.Mock
 * Reglas:     RNF-15
 * Estado:     Completo
 */

using System.Net.Http.Json;
using Acompana.Application.Common;
using Acompana.Application.Integrations;

namespace Acompana.Infrastructure.Integrations;

public sealed class InventoryClient : IInventoryClient
{
    private readonly HttpClient _http;
    public InventoryClient(HttpClient http) => _http = http;


    public async Task<int> ReportMovementAsync(long branchId, string sku, int delta, CancellationToken ct)
    {
        try
        {
            using var res = await _http.PostAsJsonAsync("/inventario/movements", new { branchId, sku, delta }, ct);
            res.EnsureSuccessStatusCode();
            using var doc = await System.Text.Json.JsonDocument.ParseAsync(await res.Content.ReadAsStreamAsync(ct), cancellationToken: ct);
            return doc.RootElement.GetProperty("balance").GetInt32();
        }
        catch (Exception ex) when (ex is HttpRequestException or TaskCanceledException or Polly.Timeout.TimeoutRejectedException)
        {
            throw new IntegrationUnavailableException("El sistema de inventario no responde.", ex);
        }
    }

    public async Task<string> RequestRestockAsync(long branchId, string sku, int qty, CancellationToken ct)
    {
        try
        {
            using var res = await _http.PostAsJsonAsync("/inventario/restock", new { branchId, sku, qty }, ct);
            res.EnsureSuccessStatusCode();
            using var doc = await System.Text.Json.JsonDocument.ParseAsync(await res.Content.ReadAsStreamAsync(ct), cancellationToken: ct);
            return doc.RootElement.GetProperty("requestNumber").GetString() ?? "";
        }
        catch (Exception ex) when (ex is HttpRequestException or TaskCanceledException or Polly.Timeout.TimeoutRejectedException)
        {
            throw new IntegrationUnavailableException("La bodega central no responde.", ex);
        }
    }
}
