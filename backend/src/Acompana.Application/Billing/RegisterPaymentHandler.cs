/*
 * Archivo:    backend/src/Acompana.Application/Billing/RegisterPaymentHandler.cs
 * Qué hace:   Registra el pago de una factura aceptada y arranca la operación del servicio.
 * Recibe:     invoiceId + PayInvoiceRequest.
 * Procesa:    1) exige Invoice.Status = Accepted y monto = total
 *             2) crea Payment; Service.ChangeStatus(Paid)
 *             3) DeductCasketHandler (stock + inventario externo, RF-26/27/28)
 *             4) DispatchOffersHandler (ofertas a proveedores, RF-15)
 *             todo en una transacción; las llamadas externas después del commit
 * Devuelve:   InvoiceDto con pago; el servicio queda Paid.
 * Se conecta: ← POST /api/invoices/{id}/pay · → DeductCasketHandler, DispatchOffersHandler,
 *               IOpsNotifier
 * Reglas:     RF-08 · RF-15 · RF-26
 * Estado:     Pendiente
 *
 * Contrato:
 *   Task<InvoiceDto> HandleAsync(Guid invoiceId, PayInvoiceRequest req, CancellationToken ct)
 *     Recibe:     factura, método y monto
 *     Procesa:    pasos 1-4
 *     Devuelve:   factura pagada
 *     Errores:    422 'La factura no ha sido aceptada por el core contable.' · 422 'El monto no
 *                   coincide.'
 */
