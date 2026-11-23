/*
 * Archivo:    backend/src/Acompana.Application/Billing/BillingDtos.cs
 * Qué hace:   DTO de facturación.
 * Recibe:     —
 * Procesa:    —
 * Devuelve:   InvoiceDto { id, number, receiverName, receiverNationalId, subtotalCrc, exemptCrc,
 *               taxCrc, totalCrc, status, rejectReason?, coreReference?, sentAt?, acceptedAt?, lines[] }
 *             PayInvoiceRequest { method, amountCrc }
 *             InvoiceTotals { subtotal, exempt, tax, total } (interno)
 * Se conecta: ← BillingController · → core/models/billing.models.ts
 * Reglas:     RF-08 a RF-13
 * Estado:     Completo
 */

using Acompana.Domain.Enums;

namespace Acompana.Application.Billing;

public record InvoiceLineDto(string Description, int Qty, decimal UnitPriceCrc, int TaxExemptPct, decimal LineTotalCrc, decimal TaxCrc);

public record InvoiceDto(
    Guid Id, string Number, string ReceiverName, string ReceiverNationalId,
    decimal SubtotalCrc, decimal ExemptCrc, decimal TaxCrc, decimal TotalCrc,
    string Status, string? RejectReason, string? CoreReference,
    DateTimeOffset? SentAt, DateTimeOffset? AcceptedAt, bool Paid, DateTimeOffset? PaidAt,
    IReadOnlyList<InvoiceLineDto> Lines);

public record PayInvoiceRequest(PaymentMethod Method, decimal AmountCrc);

/// <summary>Totales calculados de una factura (interno).</summary>
public record InvoiceTotals(decimal Subtotal, decimal Exempt, decimal Tax, decimal Total);
