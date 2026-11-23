/*
 * Archivo:    backend/src/Acompana.Api/Controllers/AdminController.cs
 * Qué hace:   Acciones del modo admin del demo.
 * Recibe:     Peticiones HTTP bajo /api con X-Demo-Role y X-Demo-Branch (ya resueltos en
 *               ICurrentUser).
 * Procesa:    Sin lógica de negocio: valida forma, llama al handler/query y traduce el resultado a
 *               HTTP.
 * Devuelve:   JSON camelCase; errores como ProblemDetails con mensaje en español.
 * Se conecta: → Application/Admin/*
 * Reglas:     —
 * Estado:     Pendiente
 *
 * Contrato:
 *   POST /api/admin/reset
 *     Recibe:     —
 *     Devuelve:   200 resumen del seed
 *   POST /api/admin/refresh-analytics
 *     Recibe:     —
 *     Devuelve:   200 { refreshedAt }
 *   POST /api/admin/simulate/concurrent-accept
 *     Recibe:     —
 *     Devuelve:   200 resultado de las dos aceptaciones
 *   POST /api/admin/simulate/next-invoice-reject
 *     Recibe:     —
 *     Devuelve:   200
 *   POST /api/admin/simulate/core-down?minutes=
 *     Recibe:     minutos (5 por defecto)
 *     Devuelve:   200
 *
 * Notas:      No requiere rol: el modo admin es del presentador.
 */
