/*
 * Archivo:    backend/src/Acompana.Api/Controllers/ProvidersController.cs
 * Qué hace:   Cartera de proveedores y oportunidades abiertas.
 * Recibe:     Peticiones HTTP bajo /api con X-Demo-Role y X-Demo-Branch (ya resueltos en
 *               ICurrentUser).
 * Procesa:    Sin lógica de negocio: valida forma, llama al handler/query y traduce el resultado a
 *               HTTP.
 * Devuelve:   JSON camelCase; errores como ProblemDetails con mensaje en español.
 * Se conecta: → Application/Providers/ProvidersHandler, GetOpenOpportunitiesQuery
 * Reglas:     RF-14 · RF-17
 * Estado:     Pendiente
 *
 * Contrato:
 *   GET /api/providers?type=&zone=&active=  (RF-14)
 *     Recibe:     filtros
 *     Devuelve:   200 ProviderDto[]
 *   POST|PUT /api/providers[/{id}]
 *     Recibe:     ProviderDto
 *     Devuelve:   200/201 ProviderDto
 *   GET /api/providers/opportunities?branch=  (RF-17)
 *     Recibe:     sede
 *     Devuelve:   200 OpenOpportunityDto[]
 */
