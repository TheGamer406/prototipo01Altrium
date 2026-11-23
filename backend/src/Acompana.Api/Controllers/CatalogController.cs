/*
 * Archivo:    backend/src/Acompana.Api/Controllers/CatalogController.cs
 * Qué hace:   Endpoints del catálogo: componentes, paquetes y modelos de cofre.
 * Recibe:     Peticiones HTTP bajo /api con X-Demo-Role y X-Demo-Branch (ya resueltos en
 *               ICurrentUser).
 * Procesa:    Sin lógica de negocio: valida forma, llama al handler/query y traduce el resultado a
 *               HTTP.
 * Devuelve:   JSON camelCase; errores como ProblemDetails con mensaje en español.
 * Se conecta: → Application/Catalog/*
 * Reglas:     RF-04 · RF-06
 * Estado:     Pendiente
 *
 * Contrato:
 *   GET /api/components?category=&active=
 *     Recibe:     filtros
 *     Devuelve:   200 ComponentDto[]
 *   POST|PUT /api/components[/{id}]
 *     Recibe:     ComponentDto
 *     Devuelve:   200/201 ComponentDto · 403 si no es Gerencia
 *   GET /api/packages
 *     Recibe:     —
 *     Devuelve:   200 PackageDto[]
 *   POST|PUT /api/packages[/{id}]
 *     Recibe:     PackageDto
 *     Devuelve:   200/201 PackageDto
 *   GET /api/casket-models?branch=
 *     Recibe:     sede para incluir stock
 *     Devuelve:   200 CasketModelDto[]
 *   POST|PUT /api/casket-models[/{id}]
 *     Recibe:     CasketModelDto
 *     Devuelve:   200/201 CasketModelDto
 */
