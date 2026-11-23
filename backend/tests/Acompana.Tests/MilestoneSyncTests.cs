/*
 * Archivo:    backend/tests/Acompana.Tests/MilestoneSyncTests.cs
 * Qué hace:   Prueba xUnit: hitos marcados sin señal se sincronizan sin duplicarse.
 * Recibe:     Datos armados en el test con TestDb (PostgreSQL de Testcontainers).
 * Procesa:    el mismo lote enviado dos veces → la primera acepta, la segunda cuenta duplicados; la hora
 *               grabada en el teléfono se conserva y el traslado pasa a En ruta.
 * Devuelve:   Pasa / falla.
 * Se conecta: → RecordMilestonesHandler
 * Reglas:     RF-22 · RNF-05
 * Estado:     Completo
 */
using Acompana.Application.Logistics;
using Acompana.Domain.Enums;
using Acompana.Domain.Logistics;
using Acompana.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace Acompana.Tests;

[Collection("db")]
public class MilestoneSyncTests
{
    private readonly TestDb _db;
    public MilestoneSyncTests(TestDb db) => _db = db;

    [Fact]
    public async Task El_mismo_lote_reenviado_no_duplica_y_conserva_la_hora_del_telefono()
    {
        using var scope = _db.Provider.CreateScope();
        var svc = await _db.CreateServiceAsync(scope);
        var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        var hearse = await db.Hearses.FirstAsync(h => h.BranchId == _db.HerediaId);
        var driver = await db.Employees.FirstAsync(e => e.BranchId == _db.HerediaId && e.Role == EmployeeRole.Driver);
        var t = new Transfer { ServiceId = svc.Id, HearseId = hearse.Id, DriverId = driver.Id, Status = TransferStatus.Planned };
        db.Transfers.Add(t); await db.SaveChangesAsync();

        var recorded = DateTimeOffset.UtcNow.AddMinutes(-30);
        var batch = new MilestoneBatch(new() { new MilestoneInput(Guid.NewGuid(), "Departure", recorded, 9.99, -84.1, "Offline") });
        var handler = scope.ServiceProvider.GetRequiredService<RecordMilestonesHandler>();

        var first = await handler.HandleAsync(t.Id, batch, default);
        var second = await handler.HandleAsync(t.Id, batch, default);

        Assert.Equal(1, first.Accepted); Assert.Equal(0, first.Duplicates);
        Assert.Equal(0, second.Accepted); Assert.Equal(1, second.Duplicates);
        var rows = await db.TransferMilestones.AsNoTracking().Where(m => m.TransferId == t.Id).ToListAsync();
        Assert.Single(rows);
        Assert.True(rows[0].SyncedAt - rows[0].RecordedAt > TimeSpan.FromMinutes(29));
        Assert.Equal(MilestoneSource.Offline, rows[0].Source);
        Assert.Equal(TransferStatus.OnRoute, (await db.Transfers.AsNoTracking().FirstAsync(x => x.Id == t.Id)).Status);
    }
}
