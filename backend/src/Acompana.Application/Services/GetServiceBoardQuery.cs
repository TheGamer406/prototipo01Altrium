/*
 * Archivo:    backend/src/Acompana.Application/Services/GetServiceBoardQuery.cs
 * Qué hace:   Tablero del servicio: estado y responsable de cada componente.
 * Recibe:     id.
 * Procesa:    Líneas + proveedor adjudicado + IsUrgent(now) (< 2 h para DueAt y aún sin aceptar).
 * Devuelve:   ServiceBoardDto.
 * Se conecta: ← GET /api/services/{id}/board · se refresca con ServiceUpdated/OfferAwarded
 * Reglas:     RF-17 · RF-19
 * Estado:     Pendiente
 */
