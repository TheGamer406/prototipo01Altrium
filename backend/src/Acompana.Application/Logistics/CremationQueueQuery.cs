/*
 * Archivo:    backend/src/Acompana.Application/Logistics/CremationQueueQuery.cs
 * Qué hace:   Tablero kanban del crematorio central.
 * Recibe:     nada (crematorio único en San José).
 * Procesa:    Cremaciones no entregadas + entregadas hoy, agrupadas por estado y ordenadas por
 *               QueuePosition.
 * Devuelve:   { waiting: CremationCardDto[], inOven: [], packed: [], delivered: [] }.
 * Se conecta: ← GET /api/cremations
 * Reglas:     RF-21
 * Estado:     Pendiente
 */
