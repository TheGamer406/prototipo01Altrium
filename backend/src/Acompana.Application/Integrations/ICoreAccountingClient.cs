/*
 * Archivo:    backend/src/Acompana.Application/Integrations/ICoreAccountingClient.cs
 * Qué hace:   Cliente del core contable (emite la factura ante Hacienda).
 * Recibe:     —
 * Procesa:    Interfaz; la implementación vive en Infrastructure/Integrations.
 * Devuelve:   —
 * Se conecta: ← handlers · → Infrastructure/Integrations (HttpClient al servicio simulado)
 * Reglas:     RF-09 · RF-13 · RNF-15
 * Estado:     Pendiente
 *
 * Contrato:
 *   Task<CoreInvoiceResult> SendInvoiceAsync(CoreInvoiceRequest req, string idempotencyKey,
 *       CancellationToken ct)
 *     Recibe:     receptor (nombre, cédula), líneas (descripción, qty, precio, % exento), totales, key
 *     Procesa:    POST /core-contable/invoices con encabezado Idempotency-Key; 3 reintentos
 *                   exponenciales (Polly)
 *     Devuelve:   CoreInvoiceResult { Accepted: bool, CoreReference?: 'FE-0000001234', RejectReason?:
 *                   string }; lanza IntegrationUnavailableException si los 3 intentos fallan
 */
