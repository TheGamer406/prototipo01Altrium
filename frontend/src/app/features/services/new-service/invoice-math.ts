/**
 * Archivo:    frontend/src/app/features/services/new-service/invoice-math.ts
 * Qué hace:   Cálculo de totales del lado del cliente (espejo de InvoiceCalculator.cs).
 * Recibe:     Líneas { qty, unitPriceCrc, taxExemptPct }.
 * Procesa:    Mismas reglas de redondeo que el backend.
 * Devuelve:   { subtotal, exempt, tax, total }.
 * Se conecta: ← total-panel, detail-invoice.tab
 * Reglas:     RF-10
 * Estado:     Pendiente
 */
