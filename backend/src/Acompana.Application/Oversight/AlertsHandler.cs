/*
 * Archivo:    backend/src/Acompana.Application/Oversight/AlertsHandler.cs
 * Qué hace:   Lista y marca como vistas las alertas.
 * Recibe:     branch? · alertId para ack.
 * Procesa:    BranchScope; ack fija AcknowledgedAt.
 * Devuelve:   AlertDto[] · 204.
 * Se conecta: ← GET /api/alerts, POST /api/alerts/{id}/ack · campana de la barra superior
 * Reglas:     RF-17 · RF-24 · RF-25 · RF-27
 * Estado:     Pendiente
 */
