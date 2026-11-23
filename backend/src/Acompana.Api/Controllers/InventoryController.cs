/*
 * Archivo:    backend/src/Acompana.Api/Controllers/InventoryController.cs
 * Qué hace:   Stock de cofres y reabastecimiento.
 * Recibe:     Peticiones HTTP bajo /api con X-Demo-Role y X-Demo-Branch (ya resueltos en
 *               ICurrentUser).
 * Procesa:    Sin lógica de negocio: valida forma, llama al handler/query y traduce el resultado a
 *               HTTP.
 * Devuelve:   JSON camelCase; errores como ProblemDetails con mensaje en español.
 * Se conecta: → Application/Inventory/*
 * Reglas:     RF-26 a RF-28
 * Estado:     Pendiente
 *
 * Contrato:
 *   GET /api/stock?branch=  (RF-26, RF-27)
 *     Recibe:     sede
 *     Devuelve:   200 StockRowDto[]
 *   GET /api/restock-requests?status=  (RF-28)
 *     Recibe:     estado
 *     Devuelve:   200 RestockRequestDto[]
 *   POST /api/restock-requests/{id}/dispatch
 *     Recibe:     id
 *     Devuelve:   200 RestockRequestDto
 */
