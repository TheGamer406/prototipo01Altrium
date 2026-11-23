/*
 * Archivo:    backend/src/Acompana.Api/Middleware/ErrorHandlingMiddleware.cs
 * Qué hace:   Traduce excepciones a ProblemDetails en español.
 * Recibe:     Excepciones del pipeline.
 * Procesa:    DomainException → 422 · ValidationException → 400 con errors por campo · NotFound → 404
 *               · DbUpdateConcurrencyException → 409 · IntegrationUnavailableException → 503 · otras →
 *               500 genérico.
 * Devuelve:   application/problem+json { title, detail (mensaje en español), status, errors? }.
 * Se conecta: → frontend error.interceptor.ts (muestra detail tal cual)
 * Reglas:     —
 * Estado:     Completo
 */

using Acompana.Application.Common;
using Acompana.Domain.Common;
using Microsoft.EntityFrameworkCore;

namespace Acompana.Api.Middleware;

public sealed class ErrorHandlingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ErrorHandlingMiddleware> _log;

    public ErrorHandlingMiddleware(RequestDelegate next, ILogger<ErrorHandlingMiddleware> log) { _next = next; _log = log; }

    public async Task InvokeAsync(HttpContext http)
    {
        try { await _next(http); }
        catch (Exception ex) when (!http.Response.HasStarted)
        {
            var (status, title, detail, errors) = Map(ex);
            if (status >= 500) _log.LogError(ex, "Error no controlado");
            http.Response.Clear();
            http.Response.StatusCode = status;
            http.Response.ContentType = "application/problem+json";
            await http.Response.WriteAsJsonAsync(new { title, detail, status, errors }, (System.Text.Json.JsonSerializerOptions?)null, "application/problem+json");
        }
    }

    private static (int, string, string, IReadOnlyDictionary<string, string[]>?) Map(Exception ex) => ex switch
    {
        DomainException d => (422, "Regla de negocio", d.Message, null),
        Application.Common.ValidationException v => (400, "Datos no válidos", v.Message, v.Errors),
        NotFoundException n => (404, "No encontrado", n.Message, null),
        ForbiddenException f => (403, "Sin permiso", f.Message, null),
        ConflictException c => (409, "Conflicto", c.Message, null),
        GoneException g => (410, "Ya no disponible", g.Message, null),
        DbUpdateConcurrencyException => (409, "Conflicto", "Otro usuario modificó este registro; recargue.", null),
        IntegrationUnavailableException i => (503, "Servicio externo no disponible", i.Message, null),
        _ => (500, "Error interno", "Ocurrió un error inesperado. Intente de nuevo.", null),
    };
}
