/*
 * Archivo:    backend/tests/Acompana.Tests/PaymentBeforeStartTests.cs
 * Qué hace:   Prueba xUnit: Cobro antes de iniciar.
 * Recibe:     Datos armados en el test con TestDb (PostgreSQL de Testcontainers).
 * Procesa:    sin pago no se despachan ofertas ni se asigna personal; tras pagar sí
 * Devuelve:   Pasa / falla.
 * Se conecta: → handlers de Application
 * Reglas:     RF-08
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
public class PaymentBeforeStartTests
{
    private readonly TestDb _db;
    public PaymentBeforeStartTests(TestDb db) => _db = db;

    [Fact]
    public async Task Sin_pago_no_hay_ofertas_ni_inicio_y_tras_pagar_se_despachan()
    {
        using var scope = _db.Provider.CreateScope();
        var svc = await _db.CreateServiceAsync(scope);
        var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        db.DeathCertificates.Add(new DeathCertificate { ServiceId = svc.Id, Number = "ACT-2", FilePath = "x", RegisteredBy = "Pruebas", RegisteredAt = DateTimeOffset.UtcNow });
        await db.SaveChangesAsync();
        var inv = await scope.ServiceProvider.GetRequiredService<IssueInvoiceHandler>().HandleAsync(svc.Id, default);

        Assert.Equal(0, await db.ProviderOffers.CountAsync(o => o.ServiceLine!.ServiceId == svc.Id));
        var tracked = await db.Services.FirstAsync(s => s.Id == svc.Id);
        var ex = Assert.Throws<DomainException>(() => tracked.ChangeStatus(ServiceStatus.InProgress, "Pruebas", DateTimeOffset.UtcNow));
        Assert.Equal("El servicio no puede iniciar sin factura pagada.", ex.Message);

        await scope.ServiceProvider.GetRequiredService<RegisterPaymentHandler>().HandleAsync(inv.Id, new PayInvoiceRequest(PaymentMethod.Card, inv.TotalCrc), default);
        Assert.True(await db.ProviderOffers.CountAsync(o => o.ServiceLine!.ServiceId == svc.Id) > 0);
        Assert.Equal(ServiceStatus.Paid, (await db.Services.AsNoTracking().FirstAsync(s => s.Id == svc.Id)).Status);
    }

    [Fact]
    public async Task El_monto_debe_coincidir_con_el_total()
    {
        using var scope = _db.Provider.CreateScope();
        var svc = await _db.CreateServiceAsync(scope);
        var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        db.DeathCertificates.Add(new DeathCertificate { ServiceId = svc.Id, Number = "ACT-3", FilePath = "x", RegisteredBy = "Pruebas", RegisteredAt = DateTimeOffset.UtcNow });
        await db.SaveChangesAsync();
        var inv = await scope.ServiceProvider.GetRequiredService<IssueInvoiceHandler>().HandleAsync(svc.Id, default);
        var ex = await Assert.ThrowsAsync<DomainException>(() => scope.ServiceProvider.GetRequiredService<RegisterPaymentHandler>().HandleAsync(inv.Id, new PayInvoiceRequest(PaymentMethod.Cash, 1m), default));
        Assert.Equal("El monto no coincide.", ex.Message);
    }
}
