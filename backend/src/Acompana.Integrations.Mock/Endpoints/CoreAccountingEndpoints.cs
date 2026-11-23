/*
 * Archivo:    backend/src/Acompana.Integrations.Mock/Endpoints/CoreAccountingEndpoints.cs
 * Qué hace:   Simula el core contable que emite la factura ante Hacienda.
 * Recibe:     POST /core-contable/invoices con Idempotency-Key y { receiver{name, nationalId},
 *               lines[], totals }.
 * Procesa:    si CoreDownUntil > now → 503
 *             si la key ya existe → misma respuesta guardada
 *             si RejectNext o cédula termina en 9 → Rejected 'Identificación del receptor inválida'
 *             si no → Accepted con coreReference FE-0000001234 (consecutivo)
 * Devuelve:   200 { status: 'Accepted'|'Rejected', coreReference?, rejectReason? } · 503.
 * Se conecta: ← CoreAccountingClient
 * Reglas:     RF-09 · RF-13
 * Estado:     Pendiente
 */
