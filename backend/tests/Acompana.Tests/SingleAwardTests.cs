/*
 * Archivo:    backend/tests/Acompana.Tests/SingleAwardTests.cs
 * Qué hace:   Prueba xUnit: Adjudicación única.
 * Recibe:     Datos armados en el test con TestDb (PostgreSQL de Testcontainers).
 * Procesa:    dos AcceptOfferHandler en paralelo sobre la misma línea → exactamente uno won=true, otro
 *               'Ya fue asignada'
 * Devuelve:   Pasa / falla.
 * Se conecta: → handlers de Application
 * Reglas:     RF-16 · RNF-16
 * Estado:     Completo
 */

using Acompana.Application.Billing;
using Acompana.Application.Providers;
using Acompana.Domain.Enums;
using Acompana.Domain.Services;
using Acompana.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace Acompana.Tests;

[Collection("db")]
public class SingleAwardTests
{
    private readonly TestDb _db;
    public SingleAwardTests(TestDb db) => _db = db;

    [Fact]
    public async Task Dos_aceptaciones_simultaneas_adjudican_a_uno_solo()
    {
        Guid serviceId; long p1, p2; Guid o1, o2;
        using (var scope = _db.Provider.CreateScope())
        {
            var svc = await _db.CreateServiceAsync(scope);
            serviceId = svc.Id;
            var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
            db.DeathCertificates.Add(new DeathCertificate { ServiceId = svc.Id, Number = "ACT-4", FilePath = "x", RegisteredBy = "Pruebas", RegisteredAt = DateTimeOffset.UtcNow });
            await db.SaveChangesAsync();
            var inv = await scope.ServiceProvider.GetRequiredService<IssueInvoiceHandler>().HandleAsync(svc.Id, default);
            await scope.ServiceProvider.GetRequiredService<RegisterPaymentHandler>().HandleAsync(inv.Id, new PayInvoiceRequest(PaymentMethod.Card, inv.TotalCrc), default);
            var line = await db.ServiceLines.Include(l => l.Component).FirstAsync(l => l.ServiceId == svc.Id && l.Status == LineStatus.Offered && l.Component!.Category == ComponentCategory.Flowers);
            var offers = await db.ProviderOffers.Where(o => o.ServiceLineId == line.Id).OrderBy(o => o.ProviderId).Take(2).ToListAsync();
            (p1, o1) = (offers[0].ProviderId, offers[0].Id); (p2, o2) = (offers[1].ProviderId, offers[1].Id);
        }

        async Task<AcceptOfferResult> Accept(long p, Guid o)
        {
            using var scope = _db.Provider.CreateScope();
            return await scope.ServiceProvider.GetRequiredService<AcceptOfferHandler>().HandleAsync(o, p, default);
        }
        var results = await Task.WhenAll(Accept(p1, o1), Accept(p2, o2));

        Assert.Equal(1, results.Count(r => r.Won));
        Assert.Equal(1, results.Count(r => !r.Won && r.Message == "Ya fue asignada"));
        using var check = _db.Provider.CreateScope();
        var cdb = check.ServiceProvider.GetRequiredService<AppDbContext>();
        var accepted = await cdb.ServiceLines.AsNoTracking().Where(l => l.ServiceId == serviceId && l.Status == LineStatus.Accepted && l.Component!.Category == ComponentCategory.Flowers).ToListAsync();
        Assert.Single(accepted);
        Assert.Equal(1, await cdb.ProviderOffers.CountAsync(o => o.ServiceLineId == accepted[0].Id && o.Status == OfferStatus.Won));
    }
}
