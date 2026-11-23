/*
 * Archivo:    backend/src/Acompana.Domain/Common/DomainException.cs
 * Qué hace:   Excepción de regla de negocio violada dentro del dominio.
 * Recibe:     Mensaje en español listo para mostrar y código corto (p. ej. `death-certificate-
 *               required`).
 * Procesa:    Nada; solo transporta el error.
 * Devuelve:   La atrapa ErrorHandlingMiddleware y la convierte en ProblemDetails 422.
 * Se conecta: ← entidades y handlers · → Api/Middleware/ErrorHandlingMiddleware
 * Reglas:     Errores en español que la interfaz muestra tal cual
 * Estado:     Pendiente
 */
