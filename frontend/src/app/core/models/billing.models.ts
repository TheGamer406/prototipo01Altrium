/**
 * Archivo:    frontend/src/app/core/models/billing.models.ts
 * Qué hace:   Tipos TypeScript: Invoice, PayInvoiceRequest (espejo de BillingDtos.cs).
 * Recibe:     —
 * Procesa:    Solo interfaces y tipos unión; deben coincidir con los DTO del backend.
 * Devuelve:   —
 * Se conecta: ← core/api/billing.api.ts y features
 * Estado:     Completo
 */

export interface InvoiceLine { description: string; qty: number; unitPriceCrc: number; taxExemptPct: number; lineTotalCrc: number; taxCrc: number; }
export interface Invoice {
  id: string; number: string; receiverName: string; receiverNationalId: string;
  subtotalCrc: number; exemptCrc: number; taxCrc: number; totalCrc: number;
  status: 'Draft' | 'Sent' | 'Accepted' | 'Rejected'; rejectReason: string | null; coreReference: string | null;
  sentAt: string | null; acceptedAt: string | null; paid: boolean; paidAt: string | null; lines: InvoiceLine[];
}
export type PaymentMethod = 'Card' | 'Transfer' | 'Cash' | 'Insurer';
export interface PayInvoiceRequest { method: PaymentMethod; amountCrc: number; }
