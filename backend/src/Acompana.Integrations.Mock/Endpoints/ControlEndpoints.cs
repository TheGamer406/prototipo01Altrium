/*
 * Archivo:    backend/src/Acompana.Integrations.Mock/Endpoints/ControlEndpoints.cs
 * Qué hace:   Control del simulador desde el modo admin.
 * Recibe:     POST /_control/core-down?minutes= · POST /_control/reject-next.
 * Procesa:    Fija CoreDownUntil = now + minutos · RejectNext = true (se consume en la próxima
 *               factura).
 * Devuelve:   200.
 * Se conecta: ← MockControlClient
 * Estado:     Completo
 */

namespace Acompana.Integrations.Mock.Endpoints;

public static class ControlEndpoints
{
    public static void Map(WebApplication app)
    {
        app.MapPost("/_control/core-down", (int? minutes, MockState state) =>
        {
            state.CoreDownUntil = DateTimeOffset.UtcNow.AddMinutes(minutes ?? 5);
            return Results.Ok(new { coreDownUntil = state.CoreDownUntil });
        });
        app.MapPost("/_control/reject-next", (MockState state) =>
        {
            state.RejectNext = true;
            return Results.Ok();
        });
        app.MapPost("/_control/reset", (MockState state) =>
        {
            state.CoreDownUntil = DateTimeOffset.MinValue;
            state.RejectNext = false;
            return Results.Ok();
        });
    }
}
