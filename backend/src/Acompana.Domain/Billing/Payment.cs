/*
 * Archivo:    backend/src/Acompana.Domain/Billing/Payment.cs
 * Qué hace:   Entidad Payment, se guarda en la tabla `payments`.
 * Recibe:     Campos: InvoiceId, Method (PaymentMethod), AmountCrc, PaidAt. Los llena el handler que
 *               la crea o EF Core al leer.
 * Procesa:    AmountCrc debe ser igual al total de la factura (sin pagos parciales en el prototipo).
 * Devuelve:   Nada; es estado. Hereda Id, CreatedAt y UpdatedAt de Entity.
 * Se conecta: ← RegisterPaymentHandler
 * Reglas:     RF-08
 * Estado:     Completo
 */

using Acompana.Domain.Common;
using Acompana.Domain.Enums;

namespace Acompana.Domain.Billing;

public class Payment : GuidEntity
{
    public Guid InvoiceId { get; set; }
    public PaymentMethod Method { get; set; }
    public decimal AmountCrc { get; set; }
    public DateTimeOffset PaidAt { get; set; }
}
