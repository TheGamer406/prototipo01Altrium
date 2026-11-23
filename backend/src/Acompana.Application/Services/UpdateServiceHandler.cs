/*
 * Archivo:    backend/src/Acompana.Application/Services/UpdateServiceHandler.cs
 * Qué hace:   Edita datos generales del servicio (familia, velación, destino, fecha, pagador).
 * Recibe:     id + UpdateServiceRequest (mismos campos editables que la creación, sin líneas) +
 *               Version.
 * Procesa:    Carga, compara Version (concurrencia optimista), aplica cambios; no permite editar si
 *               Status ≥ Paid salvo fecha y dirección.
 * Devuelve:   ServiceDetailDto actualizado.
 * Se conecta: ← PUT /api/services/{id} · → AuditInterceptor registra antes/después
 * Reglas:     RF-01 · RF-07 · RF-31
 * Estado:     Pendiente
 *
 * Contrato:
 *   Task<ServiceDetailDto> HandleAsync(Guid id, UpdateServiceRequest req, CancellationToken ct)
 *     Recibe:     id, cambios, versión
 *     Procesa:    ver arriba
 *     Devuelve:   detalle
 *     Errores:    404 · 409 'Otro usuario modificó este servicio; recargue.'
 */
