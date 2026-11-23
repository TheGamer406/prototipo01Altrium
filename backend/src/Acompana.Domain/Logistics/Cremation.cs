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
 * Estado:     Completo
 *
 * Contrato:
 *   void Advance(long operatorId, DateTimeOffset now, string? receivedBy)
 *     Recibe:     operador que mueve la tarjeta, hora, quién recibe (solo en el último paso)
 *     Procesa:    Status = siguiente(Status); fija InOvenAt / PackedAt / DeliveredAt según corresponda
 *     Devuelve:   void
 *     Errores:    'La cremación ya fue entregada.' · 'Indique quién recibe las cenizas.'
 */

using Acompana.Domain.Common;
using Acompana.Domain.Enums;

namespace Acompana.Domain.Logistics;

public class Cremation : Entity
{
    public Guid ServiceId { get; set; }
    public int QueuePosition { get; set; }
    public CremationStatus Status { get; set; } = CremationStatus.Waiting;
    public DateTimeOffset WaitingAt { get; set; }
    public DateTimeOffset? InOvenAt { get; set; }
    public DateTimeOffset? PackedAt { get; set; }
    public DateTimeOffset? DeliveredAt { get; set; }
    public long? OperatorId { get; set; }
    public string? ReceivedBy { get; set; }

    public void Advance(long operatorId, DateTimeOffset now, string? receivedBy)
    {
        switch (Status)
        {
            case CremationStatus.Waiting: Status = CremationStatus.InOven; InOvenAt = now; break;
            case CremationStatus.InOven: Status = CremationStatus.Packed; PackedAt = now; break;
            case CremationStatus.Packed:
                if (string.IsNullOrWhiteSpace(receivedBy))
                    throw new DomainException("Indique quién recibe las cenizas.", "receiver-required");
                Status = CremationStatus.Delivered; DeliveredAt = now; ReceivedBy = receivedBy.Trim(); break;
            default:
                throw new DomainException("La cremación ya fue entregada.", "cremation-delivered");
        }
        OperatorId = operatorId;
    }
}
