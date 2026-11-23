/*
 * Archivo:    backend/src/Acompana.Domain/Logistics/Cremation.cs
 * Qué hace:   Entidad Cremation (tabla `cremations`): un cuerpo en la cola del crematorio central.
 * Recibe:     Campos: ServiceId, QueuePosition, Status, WaitingAt, InOvenAt?, PackedAt?, DeliveredAt?,
 *               OperatorId?, ReceivedBy?.
 * Procesa:    Advance(operatorId, now, receivedBy?): solo Waiting→InOven→Packed→Delivered, guarda la
 *               hora del paso y el operador; al entregar exige receivedBy.
 * Devuelve:   void; DomainException si se intenta saltar o retroceder un estado.
 * Se conecta: ← AdvanceCremationHandler · → correo a la familia al entregar
 * Reglas:     RF-21
 * Estado:     Pendiente
 *
 * Contrato:
 *   void Advance(long operatorId, DateTimeOffset now, string? receivedBy)
 *     Recibe:     operador que mueve la tarjeta, hora, quién recibe (solo en el último paso)
 *     Procesa:    Status = siguiente(Status); fija InOvenAt / PackedAt / DeliveredAt según corresponda
 *     Devuelve:   void
 *     Errores:    'La cremación ya fue entregada.' · 'Indique quién recibe las cenizas.'
 */
