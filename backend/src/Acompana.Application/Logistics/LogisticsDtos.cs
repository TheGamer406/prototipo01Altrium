/*
 * Archivo:    backend/src/Acompana.Application/Logistics/LogisticsDtos.cs
 * Qué hace:   DTO de capillas, personal, traslados y cremación.
 * Recibe:     —
 * Procesa:    —
 * Devuelve:   ChapelCalendarDto { chapels[{id, name, capacity, bookings[{serviceCode, from, to}]}] }
 *             BookChapelRequest { chapelId, serviceId, from, to }
 *             AssignStaffRequest { serviceId, employeeId, from, to } → AssignStaffResult { id,
 *               overlapWith?: serviceCode }
 *             TransferDto { id, serviceCode, plate, driverName, status, lastLat?, lastLng?,
 *               milestones[], delayed }
 *             MilestoneBatch { items[{clientUuid, type, recordedAt, lat?, lng?, source}] } → {
 *               accepted, duplicates }
 *             CremationCardDto { id, serviceCode, deceasedName, status, queuePosition, times{...},
 *               operator? }
 * Se conecta: ← LogisticsController · → core/models/logistics.models.ts
 * Reglas:     RF-20 a RF-25
 * Estado:     Pendiente
 */
