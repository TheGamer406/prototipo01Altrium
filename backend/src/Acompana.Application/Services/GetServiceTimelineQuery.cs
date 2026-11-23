/*
 * Archivo:    backend/src/Acompana.Application/Services/GetServiceTimelineQuery.cs
 * Qué hace:   Línea de tiempo del servicio (estados, cambios y correos).
 * Recibe:     id.
 * Procesa:    Une StatusHistory + AuditLog del servicio + OutboundMessages y ordena por fecha.
 * Devuelve:   TimelineEntryDto[] ordenado ascendente.
 * Se conecta: ← GET /api/services/{id}/timeline
 * Reglas:     RF-19 · RF-31 · RF-35
 * Estado:     Pendiente
 */
