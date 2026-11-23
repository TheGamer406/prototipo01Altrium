/*
 * Archivo:    backend/src/Acompana.Domain/Billing/Invoice.cs
 * Qué hace:   Entidad Invoice, se guarda en la tabla `invoices`.
 * Recibe:     Campos: ServiceId, Number, ReceiverName, ReceiverNationalId, SubtotalCrc, ExemptCrc,
 *               TaxCrc, TotalCrc, Status, RejectReason?, CoreReference?, IdempotencyKey (único),
 *               SentAt?, AcceptedAt?. Los llena el handler que la crea o EF Core al leer.
 * Procesa:    MarkSent / MarkAccepted(coreRef) / MarkRejected(reason). Un servicio tiene a lo sumo una
 *               factura Accepted.
 * Devuelve:   Nada; es estado. Hereda Id, CreatedAt y UpdatedAt de Entity.
 * Se conecta: ← IssueInvoiceHandler · → Payment, core contable simulado
 * Reglas:     RF-09 · RF-11 · RF-13 · RNF-15
 * Estado:     Completo
 */

using Acompana.Domain.Common;
using Acompana.Domain.Enums;

namespace Acompana.Domain.Billing;

public class Invoice : GuidEntity
{
    public Guid ServiceId { get; set; }
    public string Number { get; set; } = "";
    public string ReceiverName { get; set; } = "";
    public string ReceiverNationalId { get; set; } = "";
    public decimal SubtotalCrc { get; set; }
    public decimal ExemptCrc { get; set; }
    public decimal TaxCrc { get; set; }
    public decimal TotalCrc { get; set; }
    public InvoiceStatus Status { get; set; } = InvoiceStatus.Draft;
    public string? RejectReason { get; set; }
    public string? CoreReference { get; set; }
    public string IdempotencyKey { get; set; } = "";
    public DateTimeOffset? SentAt { get; set; }
    public DateTimeOffset? AcceptedAt { get; set; }
    public List<Payment> Payments { get; set; } = new();

    public void MarkSent(DateTimeOffset now) { Status = InvoiceStatus.Sent; SentAt = now; RejectReason = null; }

    public void MarkAccepted(string coreRef, DateTimeOffset now)
    {
        Status = InvoiceStatus.Accepted; CoreReference = coreRef; AcceptedAt = now; RejectReason = null;
    }

    public void MarkRejected(string reason) { Status = InvoiceStatus.Rejected; RejectReason = reason; }
}
