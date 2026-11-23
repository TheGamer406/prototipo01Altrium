/*
 * Archivo:    backend/src/Acompana.Application/Services/ActivateAdvanceServiceHandler.cs
 * Qué hace:   Activa un servicio anticipado cuando ocurre el fallecimiento.
 * Recibe:     id + { deathDate, scheduledAt }.
 * Procesa:    Verifica Kind = Advance y Status = Advance; fija DeathDate; ChangeStatus(Registered);
 *               conserva paquete y datos (no se repiten).
 * Devuelve:   ServiceDetailDto.
 * Se conecta: ← POST /api/services/{id}/activate
 * Reglas:     RF-02
 * Estado:     Pendiente
 *
 * Contrato:
 *   Task<ServiceDetailDto> HandleAsync(Guid id, ActivateRequest req, CancellationToken ct)
 *     Recibe:     id, fecha de defunción, fecha del servicio
 *     Procesa:    ver arriba
 *     Devuelve:   detalle
 *     Errores:    422 'El servicio no es anticipado o ya fue activado.'
 */
