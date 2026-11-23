/*
 * Archivo:    backend/src/Acompana.Api/Controllers/ServicesController.cs
 * Qué hace:   Endpoints del módulo Servicios (registro y venta).
 * Recibe:     Peticiones HTTP bajo /api con X-Demo-Role y X-Demo-Branch (ya resueltos en
 *               ICurrentUser).
 * Procesa:    Sin lógica de negocio: valida forma, llama al handler/query y traduce el resultado a
 *               HTTP.
 * Devuelve:   JSON camelCase; errores como ProblemDetails con mensaje en español.
 * Se conecta: → Application/Services/* (Create/Update/Activate/SetPackage/RegisterDeathCertificate,
 *               queries)
 * Reglas:     RF-01 a RF-07 · RF-19 · RF-36
 * Estado:     Pendiente
 *
 * Contrato:
 *   GET /api/services?status=&branch=&q=&page=  (RF-19, RF-30)
 *     Recibe:     filtros en query string
 *     Devuelve:   200 PagedResult<ServiceListItemDto>
 *   GET /api/services/{id}
 *     Recibe:     id (Guid)
 *     Devuelve:   200 ServiceDetailDto · 404
 *   POST /api/services  (RF-01, RF-02)
 *     Recibe:     CreateServiceRequest (JSON)
 *     Devuelve:   201 ServiceDetailDto + Location
 *   PUT /api/services/{id}
 *     Recibe:     UpdateServiceRequest
 *     Devuelve:   200 ServiceDetailDto · 409 concurrencia
 *   POST /api/services/{id}/activate  (RF-02)
 *     Recibe:     { deathDate, scheduledAt }
 *     Devuelve:   200 ServiceDetailDto
 *   PUT /api/services/{id}/package  (RF-04, RF-05)
 *     Recibe:     { basePackageId, lines[] }
 *     Devuelve:   200 ServiceDetailDto
 *   POST /api/services/{id}/death-certificate  (RF-03)
 *     Recibe:     multipart: number, file
 *     Devuelve:   200 DeathCertificateDto
 *   GET /api/services/{id}/board  (RF-19)
 *     Recibe:     id
 *     Devuelve:   200 ServiceBoardDto
 *   GET /api/services/{id}/timeline
 *     Recibe:     id
 *     Devuelve:   200 TimelineEntryDto[]
 *   GET /api/prospects/export?from=&to=  (RF-36)
 *     Recibe:     rango de fechas
 *     Devuelve:   200 text/csv
 */
