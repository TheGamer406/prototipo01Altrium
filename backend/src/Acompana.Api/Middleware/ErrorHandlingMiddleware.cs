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
 * Estado:     Pendiente
 */
