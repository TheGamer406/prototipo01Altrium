/*
 * Archivo:    backend/src/Acompana.Api/Controllers/OversightController.cs
 * Qué hace:   Vista central, bitácora, alertas, mensajes y parámetros.
 * Recibe:     Peticiones HTTP bajo /api con X-Demo-Role y X-Demo-Branch (ya resueltos en
 *               ICurrentUser).
 * Procesa:    Sin lógica de negocio: valida forma, llama al handler/query y traduce el resultado a
 *               HTTP.
 * Devuelve:   JSON camelCase; errores como ProblemDetails con mensaje en español.
 * Se conecta: → Application/Oversight/*
 * Reglas:     RF-29 · RF-30 · RF-31
 * Estado:     Pendiente
 *
 * Contrato:
 *   GET /api/overview  (RF-29)
 *     Recibe:     —
 *     Devuelve:   200 OverviewDto
 *   GET /api/audit?entity=&from=&to=  (RF-31)
 *     Recibe:     filtros
 *     Devuelve:   200 PagedResult<AuditEntryDto>
 *   GET /api/alerts?branch=
 *     Recibe:     sede
 *     Devuelve:   200 AlertDto[]
 *   POST /api/alerts/{id}/ack
 *     Recibe:     id
 *     Devuelve:   204
 *   GET /api/messages?serviceId=
 *     Recibe:     servicio
 *     Devuelve:   200 PagedResult<MessageDto>
 *   GET|PUT /api/settings  (RF-17, RF-24)
 *     Recibe:     { key: value }
 *     Devuelve:   200 diccionario
 */
