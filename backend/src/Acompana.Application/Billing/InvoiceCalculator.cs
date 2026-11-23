/*
 * Archivo:    backend/src/Acompana.Application/Billing/InvoiceCalculator.cs
 * Qué hace:   Calcula IVA 13 % con exoneraciones por línea.
 * Recibe:     Líneas del servicio (qty, unitPriceCrc, taxExemptPct).
 * Procesa:    por línea: base = qty × unit; gravable = base × (1 − exento/100); iva = gravable × 0,13
 *             redondeo a colones enteros (MidpointRounding.AwayFromZero) por línea, luego se suma
 * Devuelve:   InvoiceTotals { Subtotal = Σ base, Exempt = Σ (base − gravable), Tax = Σ iva, Total =
 *               Subtotal + Tax }.
 * Se conecta: ← IssueInvoiceHandler, asistente (vista previa vía /package) · probado en
 *               TaxExemptionTests
 * Reglas:     RF-10
 * Estado:     Pendiente
 *
 * Contrato:
 *   InvoiceTotals Calculate(IEnumerable<(int Qty, decimal UnitPrice, int ExemptPct)> lines)
 *     Recibe:     líneas
 *     Procesa:    ver arriba
 *     Devuelve:   totales
 */
