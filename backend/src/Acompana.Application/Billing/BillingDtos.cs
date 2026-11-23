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
 * Estado:     Pendiente
 */
