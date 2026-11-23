/*
 * Archivo:    backend/src/Acompana.Application/Billing/IssueInvoiceHandler.cs
 * Qué hace:   Arma la factura del servicio con IVA y exoneraciones y la envía al core contable.
 * Recibe:     serviceId + ICurrentUser.
 * Procesa:    1) carga servicio y líneas; exige acta si es inmediato (RF-03)
 *             2) receptor = familia, o aseguradora si Payer = Insurer (RF-11)
 *             3) InvoiceCalculator; crea/reutiliza Invoice Draft con IdempotencyKey = serviceId +
 *               intento
 *             4) Status Sent; ChangeStatus(Invoicing); notifica ServiceUpdated ('Enviando…')
 *             5) ICoreAccountingClient.SendInvoiceAsync → Accepted (CoreReference) o Rejected (motivo
 *               + alerta InvoiceRejected)
 *             6) si aseguradora: IInsurerClient.SendNoticeAsync (RF-12); correo a la familia con la
 *               factura
 * Devuelve:   InvoiceDto con el estado final.
 * Se conecta: ← BillingController POST /api/services/{id}/invoice · → ICoreAccountingClient,
 *               IInsurerClient, FamilyNotifier, AppDbContext (invoices, service_lines), IOpsNotifier
 * Reglas:     RF-03 · RF-09 · RF-10 · RF-11 · RF-12 · RF-13 (Idempotency-Key)
 * Estado:     Pendiente
 *
 * Contrato:
 *   Task<InvoiceDto> HandleAsync(Guid serviceId, CancellationToken ct)
 *     Recibe:     id del servicio
 *     Procesa:    pasos 1-6
 *     Devuelve:   factura
 *     Errores:    422 'Registre el acta de defunción antes de facturar.' · 409 'El servicio ya tiene
 *                   factura aceptada.' · 503 'El core contable no responde; se reintentará.'
 */
