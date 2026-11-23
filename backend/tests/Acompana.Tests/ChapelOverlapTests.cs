/*
 * Archivo:    backend/tests/Acompana.Tests/ChapelOverlapTests.cs
 * Qué hace:   Prueba xUnit: Capilla sin traslape.
 * Recibe:     Datos armados en el test con TestDb (PostgreSQL de Testcontainers).
 * Procesa:    dos reservas que se cruzan en la misma capilla → la segunda falla con 409 y mensaje con
 *               la reserva que choca
 * Devuelve:   Pasa / falla.
 * Se conecta: → handlers de Application
 * Reglas:     RF-20
 * Estado:     Completo
 */

using Acompana.Application.Common;
using Acompana.Domain.Logistics;
using Acompana.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace Acompana.Tests;

[Collection("db")]
public class ChapelOverlapTests
{
    private readonly TestDb _db;
    public ChapelOverlapTests(TestDb db) => _db = db;

    [Fact]
    public async Task Dos_reservas_que_se_cruzan_en_la_misma_capilla_fallan_con_409_y_mensaje()
    {
        using var scope = _db.Provider.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        var chapel = await db.Chapels.FirstAsync(c => c.BranchId == _db.HerediaId);
        var at = DateTimeOffset.UtcNow.AddDays(20);

        var first = await _db.CreateServiceAsync(scope, chapel.Id, at);
        Assert.Equal(chapel.Id, first.Wake.ChapelId);

        var ex = await Assert.ThrowsAsync<ConflictException>(() => _db.CreateServiceAsync(scope, chapel.Id, at.AddHours(5)));
        Assert.Contains($"está reservada para {first.Code}", ex.Message);
        Assert.StartsWith($"La capilla {chapel.Name}", ex.Message);
    }

    [Fact]
    public async Task La_base_de_datos_rechaza_el_traslape_aunque_se_salte_la_validacion()
    {
        using var scope = _db.Provider.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        var chapel = await db.Chapels.OrderByDescending(c => c.Id).FirstAsync(c => c.BranchId == _db.HerediaId);
        var s1 = await _db.CreateServiceAsync(scope);
        var s2 = await _db.CreateServiceAsync(scope);
        var t = DateTimeOffset.UtcNow.AddDays(40);
        db.ChapelBookings.Add(new ChapelBooking { ChapelId = chapel.Id, ServiceId = s1.Id, StartsAt = t, EndsAt = t.AddHours(24) });
        await db.SaveChangesAsync();
        db.ChapelBookings.Add(new ChapelBooking { ChapelId = chapel.Id, ServiceId = s2.Id, StartsAt = t.AddHours(2), EndsAt = t.AddHours(10) });
        await Assert.ThrowsAsync<DbUpdateException>(() => db.SaveChangesAsync());
    }
}
