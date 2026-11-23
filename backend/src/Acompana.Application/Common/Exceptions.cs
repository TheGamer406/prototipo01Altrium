/*
 * Archivo:    backend/src/Acompana.Application/Common/Exceptions.cs
 * Qué hace:   Excepciones de aplicación que ErrorHandlingMiddleware traduce a códigos HTTP.
 * Recibe:     Mensaje en español (y errores por campo en ValidationException).
 * Procesa:    Solo transportan el error.
 * Devuelve:   400 (ValidationException) · 403 (ForbiddenException) · 404 (NotFoundException) ·
 *               409 (ConflictException) · 410 (GoneException).
 * Se conecta: ← handlers y queries · → Api/Middleware/ErrorHandlingMiddleware
 * Estado:     Completo
 */
namespace Acompana.Application.Common;

public class ValidationException : Exception
{
    public IReadOnlyDictionary<string, string[]> Errors { get; }

    public ValidationException(IDictionary<string, string> errors)
        : base("Revise los campos marcados.")
    {
        Errors = errors.ToDictionary(e => e.Key, e => new[] { e.Value });
    }

    public ValidationException(string field, string message) : this(new Dictionary<string, string> { [field] = message }) { }
}

public class NotFoundException : Exception { public NotFoundException(string message = "No se encontró el registro.") : base(message) { } }

public class ConflictException : Exception { public ConflictException(string message) : base(message) { } }

public class ForbiddenException : Exception { public ForbiddenException(string message = "Su rol no tiene permiso para esta acción.") : base(message) { } }

public class GoneException : Exception { public GoneException(string message) : base(message) { } }
