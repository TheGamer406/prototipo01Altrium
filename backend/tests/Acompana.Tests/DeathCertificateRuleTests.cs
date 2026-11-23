/*
 * Archivo:    backend/tests/Acompana.Tests/DeathCertificateRuleTests.cs
 * Qué hace:   Prueba xUnit: Acta antes de facturar.
 * Recibe:     Datos armados en el test con TestDb (PostgreSQL de Testcontainers).
 * Procesa:    servicio inmediato sin acta → IssueInvoice lanza 'Registre el acta…'; con acta → pasa a
 *               Invoicing
 * Devuelve:   Pasa / falla.
 * Se conecta: → handlers de Application
 * Reglas:     RF-03
 * Estado:     Completo
 */

using Acompana.Application.Billing;
using Acompana.Domain.Common;
using Acompana.Domain.Enums;
using Acompana.Domain.Services;
using Acompana.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace Acompana.Tests;

[Collection("db")]
public class DeathCertificateRuleTests
{
    private readonly TestDb _db;
    public DeathCertificateRuleTests(TestDb db) => _db = db;

    [Fact]
    public async Task Sin_acta_no_se_puede_facturar_y_con_acta_pasa_a_facturacion()
    {
        using var scope = _db.Provider.CreateScope();
        var svc = await _db.CreateServiceAsync(scope);
        var ex = await Assert.ThrowsAsync<DomainException>(() => scope.ServiceProvider.GetRequiredService<IssueInvoiceHandler>().HandleAsync(svc.Id, default));
        Assert.Equal("Registre el acta de defunción antes de facturar.", ex.Message);

        var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        db.DeathCertificates.Add(new DeathCertificate { ServiceId = svc.Id, Number = "ACT-1", FilePath = "x", RegisteredBy = "Pruebas", RegisteredAt = DateTimeOffset.UtcNow });
        await db.SaveChangesAsync();

        var inv = await scope.ServiceProvider.GetRequiredService<IssueInvoiceHandler>().HandleAsync(svc.Id, default);
        Assert.Equal("Accepted", inv.Status);
        Assert.Equal(ServiceStatus.Invoicing, (await db.Services.AsNoTracking().FirstAsync(s => s.Id == svc.Id)).Status);
    }
}
