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
 * Estado:     Pendiente
 */
