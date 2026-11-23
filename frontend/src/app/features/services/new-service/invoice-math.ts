/**
 * Archivo:    frontend/src/app/features/services/new-service/invoice-math.ts
 * Qué hace:   Cálculo de totales del lado del cliente (espejo de InvoiceCalculator.cs).
 * Recibe:     Líneas { qty, unitPriceCrc, taxExemptPct }.
 * Procesa:    Mismas reglas de redondeo que el backend.
 * Devuelve:   { subtotal, exempt, tax, total }.
 * Se conecta: ← total-panel, detail-invoice.tab
 * Reglas:     RF-10
 * Estado:     Completo
 */

export interface MathLine { qty: number; unitPriceCrc: number; taxExemptPct: number; }
export interface Totals { subtotal: number; exempt: number; tax: number; total: number; }

const round = (n: number) => Math.floor(n + 0.5);

export function calcTotals(lines: MathLine[]): Totals {
  let subtotal = 0, exempt = 0, tax = 0;
  for (const l of lines) {
    const base = l.qty * l.unitPriceCrc;
    const taxable = base * (1 - l.taxExemptPct / 100);
    subtotal += base;
    exempt += base - taxable;
    tax += round(taxable * 0.13);
  }
  return { subtotal, exempt, tax, total: subtotal + tax };
}
