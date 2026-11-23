/*
 * Archivo:    backend/src/Acompana.Application/Billing/InvoiceMapper.cs
 * Qué hace:   Convierte Invoice (con sus pagos) y las líneas del servicio en InvoiceDto.
 * Recibe:     Invoice con Payments cargados y las líneas del servicio (con Component).
 * Procesa:    IVA por línea con InvoiceCalculator.LineTax; Paid = existe algún pago.
 * Devuelve:   InvoiceDto.
 * Se conecta: ← IssueInvoiceHandler, RegisterPaymentHandler, GetInvoiceQuery, GetServiceDetailQuery
 * Reglas:     RF-09 · RF-10
 * Estado:     Completo
 */
using Acompana.Domain.Billing;
using Acompana.Domain.Services;

namespace Acompana.Application.Billing;

public static class InvoiceMapper
{
    public static InvoiceDto ToDto(Invoice i, IEnumerable<ServiceLine> lines)
    {
        var paid = i.Payments.OrderBy(p => p.PaidAt).FirstOrDefault();
        return new InvoiceDto(i.Id, i.Number, i.ReceiverName, i.ReceiverNationalId, i.SubtotalCrc, i.ExemptCrc, i.TaxCrc, i.TotalCrc,
            i.Status.ToString(), i.RejectReason, i.CoreReference, i.SentAt, i.AcceptedAt, paid is not null, paid?.PaidAt,
            lines.Select(l => new InvoiceLineDto(l.Component?.Name ?? "Componente", l.Qty, l.UnitPriceCrc, l.TaxExemptPct, l.LineTotalCrc,
                InvoiceCalculator.LineTax(l.Qty, l.UnitPriceCrc, l.TaxExemptPct))).ToList());
    }
}
