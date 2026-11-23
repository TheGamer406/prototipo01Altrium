/**
 * Archivo:    frontend/src/app/core/http/error.interceptor.ts
 * Qué hace:   Manejo uniforme de errores HTTP.
 * Recibe:     HttpErrorResponse.
 * Procesa:    Error de red (status 0) → reintenta una vez; ProblemDetails → snackbar con detail; 400
 *               con errors → deja el error para que el formulario lo pinte por campo.
 * Devuelve:   Observable de error re-lanzado (el componente decide).
 * Se conecta: ← app.config.ts · → MatSnackBar, server-errors.ts
 * Estado:     Pendiente
 */
