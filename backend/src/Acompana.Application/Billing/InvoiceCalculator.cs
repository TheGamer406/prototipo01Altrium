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
 * Estado:     Completo
 *
 * Contrato:
 *   InvoiceTotals Calculate(IEnumerable<(int Qty, decimal UnitPrice, int ExemptPct)> lines)
 *     Recibe:     líneas
 *     Procesa:    ver arriba
 *     Devuelve:   totales
 */

namespace Acompana.Application.Billing;

public sealed class InvoiceCalculator
{
    public const decimal VatRate = 0.13m;

    /// <summary>IVA de una línea, redondeado a colones enteros.</summary>
    public static decimal LineTax(int qty, decimal unitPrice, int exemptPct)
    {
        var baseAmount = qty * unitPrice;
        var taxable = baseAmount * (1 - exemptPct / 100m);
        return Math.Round(taxable * VatRate, 0, MidpointRounding.AwayFromZero);
    }

    public InvoiceTotals Calculate(IEnumerable<(int Qty, decimal UnitPrice, int ExemptPct)> lines)
    {
        decimal subtotal = 0, exempt = 0, tax = 0;
        foreach (var (qty, unit, pct) in lines)
        {
            var baseAmount = qty * unit;
            var taxable = baseAmount * (1 - pct / 100m);
            subtotal += baseAmount;
            exempt += baseAmount - taxable;
            tax += Math.Round(taxable * VatRate, 0, MidpointRounding.AwayFromZero);
        }
        return new InvoiceTotals(subtotal, exempt, tax, subtotal + tax);
    }
}
