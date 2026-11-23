/*
 * Archivo:    backend/tests/Acompana.Tests/TaxExemptionTests.cs
 * Qué hace:   Prueba xUnit: Cálculo de exoneraciones.
 * Recibe:     Datos armados en el test con TestDb (PostgreSQL de Testcontainers).
 * Procesa:    líneas con 0 %, 50 % y 100 % exento → subtotal, exento, IVA 13 % y total esperados
 * Devuelve:   Pasa / falla.
 * Se conecta: → handlers de Application
 * Reglas:     RF-10
 * Estado:     Completo
 */

using Acompana.Application.Billing;
using Xunit;

namespace Acompana.Tests;

public class TaxExemptionTests
{
    private readonly InvoiceCalculator _calc = new();

    [Fact]
    public void Linea_gravada_paga_13_por_ciento()
    {
        var t = _calc.Calculate(new[] { (1, 100_000m, 0) });
        Assert.Equal(100_000m, t.Subtotal); Assert.Equal(0m, t.Exempt); Assert.Equal(13_000m, t.Tax); Assert.Equal(113_000m, t.Total);
    }

    [Fact]
    public void Linea_exenta_al_50_por_ciento_paga_iva_sobre_la_mitad()
    {
        var t = _calc.Calculate(new[] { (1, 100_000m, 50) });
        Assert.Equal(50_000m, t.Exempt); Assert.Equal(6_500m, t.Tax); Assert.Equal(106_500m, t.Total);
    }

    [Fact]
    public void Linea_exenta_al_100_por_ciento_no_paga_iva()
    {
        var t = _calc.Calculate(new[] { (2, 60_000m, 100) });
        Assert.Equal(120_000m, t.Exempt); Assert.Equal(0m, t.Tax); Assert.Equal(120_000m, t.Total);
    }

    [Fact]
    public void Totales_mezclados_suman_por_linea_con_redondeo_a_colones()
    {
        var t = _calc.Calculate(new[] { (1, 100_000m, 0), (1, 100_000m, 50), (1, 60_000m, 100), (1, 33m, 0) });
        Assert.Equal(260_033m, t.Subtotal);
        Assert.Equal(110_000m, t.Exempt);
        Assert.Equal(13_000m + 6_500m + 0m + 4m, t.Tax); // 33 × 0,13 = 4,29 → 4
        Assert.Equal(t.Subtotal + t.Tax, t.Total);
    }
}
