/*
 * Archivo:    backend/src/Acompana.Integrations.Mock/Endpoints/InsurerEndpoints.cs
 * Qué hace:   Simula la recepción del aviso estructurado de la aseguradora.
 * Recibe:     POST /aseguradoras/notices { insurer, policy, insured, serviceCode, amountCrc }.
 * Procesa:    Valida campos obligatorios.
 * Devuelve:   202 · 400.
 * Se conecta: ← InsurerClient
 * Reglas:     RF-12
 * Estado:     Completo
 */

namespace Acompana.Integrations.Mock.Endpoints;

public record InsurerNoticeBody(string? Insurer, string? Policy, string? Insured, string? ServiceCode, decimal AmountCrc);

public static class InsurerEndpoints
{
    public static void Map(WebApplication app)
    {
        app.MapPost("/aseguradoras/notices", async (InsurerNoticeBody b) =>
        {
            await Latency.Wait();
            if (string.IsNullOrWhiteSpace(b.Insurer) || string.IsNullOrWhiteSpace(b.Policy) || string.IsNullOrWhiteSpace(b.ServiceCode))
                return Results.BadRequest(new { error = "Faltan campos obligatorios." });
            return Results.Accepted();
        });
    }
}
