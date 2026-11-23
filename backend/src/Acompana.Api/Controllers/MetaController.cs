/*
 * Archivo:    backend/src/Acompana.Api/Controllers/MetaController.cs
 * Qué hace:   Datos de apoyo para selectores.
 * Recibe:     Peticiones HTTP bajo /api con X-Demo-Role y X-Demo-Branch (ya resueltos en
 *               ICurrentUser).
 * Procesa:    Sin lógica de negocio: valida forma, llama al handler/query y traduce el resultado a
 *               HTTP.
 * Devuelve:   JSON camelCase; errores como ProblemDetails con mensaje en español.
 * Se conecta: → Application/Meta/MetaQuery
 * Reglas:     RF-30
 * Estado:     Pendiente
 *
 * Contrato:
 *   GET /api/meta/branches
 *     Recibe:     —
 *     Devuelve:   200 sedes
 *   GET /api/meta/roles
 *     Recibe:     —
 *     Devuelve:   200 roles con etiqueta en español
 *   GET /api/meta/enums
 *     Recibe:     —
 *     Devuelve:   200 enums traducidos
 */
