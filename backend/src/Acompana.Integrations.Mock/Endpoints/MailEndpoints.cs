/*
 * Archivo:    backend/src/Acompana.Integrations.Mock/Endpoints/MailEndpoints.cs
 * Qué hace:   Simula el servidor de correo.
 * Recibe:     POST /mail/send { to, subject, body }.
 * Procesa:    No envía nada real.
 * Devuelve:   202 { messageId }.
 * Se conecta: ← MailClient
 * Reglas:     RF-35
 * Estado:     Completo
 */

namespace Acompana.Integrations.Mock.Endpoints;

public record MailBody(string To, string Subject, string Body);

public static class MailEndpoints
{
    public static void Map(WebApplication app)
    {
        app.MapPost("/mail/send", async (MailBody b) =>
        {
            await Latency.Wait();
            return Results.Accepted(value: new { messageId = Guid.NewGuid() });
        });
    }
}
