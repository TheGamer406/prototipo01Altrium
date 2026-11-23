/**
 * Archivo:    frontend/src/app/core/forms/server-errors.ts
 * Qué hace:   Pinta errores del backend en los campos del formulario.
 * Recibe:     FormGroup + ProblemDetails.errors { campo: [mensajes] }.
 * Procesa:    setErrors({ server: mensaje }) en cada control que coincida (camelCase).
 * Devuelve:   void.
 * Se conecta: ← formularios · ← error.interceptor.ts
 * Estado:     Pendiente
 */
