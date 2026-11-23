/*
 * Archivo:    backend/tests/Acompana.Tests/TestDb.cs
 * Qué hace:   Fixture de BD para pruebas.
 * Recibe:     —
 * Procesa:    Levanta PostgreSQL 16 con Testcontainers (necesario para la restricción de exclusión),
 *               migra y siembra lo mínimo.
 * Devuelve:   AppDbContext listo + helpers para crear servicios de prueba.
 * Se conecta: ← todas las pruebas
 * Estado:     Completo
 */

using Acompana.Application;
using Acompana.Application.Common;
using Acompana.Application.Integrations;
using Acompana.Application.Services;
using Acompana.Domain.Enums;
using Acompana.Infrastructure;
using Acompana.Infrastructure.Persistence;
using Acompana.Infrastructure.Seed;
using Acompana.Infrastructure.Time;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Testcontainers.PostgreSql;
using Xunit;

namespace Acompana.Tests;

public sealed class TestUser : ICurrentUser
{
    public DemoRole Role => DemoRole.Management;
    public long? BranchId => null;
    public string DisplayName => "Pruebas";
    public bool SeesAllBranches => true;
    public long? ProviderId => null;
}

public sealed class FakeCore : ICoreAccountingClient
{
    public Task<CoreInvoiceResult> SendInvoiceAsync(CoreInvoiceRequest req, string key, CancellationToken ct) =>
        Task.FromResult(req.Receiver.NationalId.EndsWith('9')
            ? new CoreInvoiceResult(false, null, "Identificación del receptor inválida")
            : new CoreInvoiceResult(true, "FE-TEST-" + Guid.NewGuid().ToString("N")[..8], null));
}

public sealed class FakeInsurer : IInsurerClient { public Task SendNoticeAsync(InsurerNotice n, CancellationToken ct) => Task.CompletedTask; }
public sealed class FakeInventory : IInventoryClient
{
    public Task<int> ReportMovementAsync(long b, string sku, int d, CancellationToken ct) => Task.FromResult(5);
    public Task<string> RequestRestockAsync(long b, string sku, int q, CancellationToken ct) => Task.FromResult("BOD-TEST");
}
public sealed class FakeMail : IMailClient { public Task SendAsync(MailMessage m, CancellationToken ct) => Task.CompletedTask; }

[CollectionDefinition("db")]
public class DbCollection : ICollectionFixture<TestDb> { }

/// <summary>PostgreSQL 16 real (Testcontainers): la restricción EXCLUDE y el UPDATE condicional lo exigen.</summary>
public sealed class TestDb : IAsyncLifetime
{
    private readonly PostgreSqlContainer _pg = new PostgreSqlBuilder().WithImage("postgres:16-alpine").Build();
    public ServiceProvider Provider { get; private set; } = default!;
    public long HerediaId { get; private set; }

    public async Task InitializeAsync()
    {
        await _pg.StartAsync();
        var config = new ConfigurationBuilder().AddInMemoryCollection(new Dictionary<string, string?> { ["ConnectionStrings:Default"] = _pg.GetConnectionString() }).Build();
        var services = new ServiceCollection();
        services.AddLogging();
        services.AddSingleton<IConfiguration>(config);
        services.AddScoped<ICurrentUser, TestUser>();
        services.AddSingleton<IOpsNotifier, NullOpsNotifier>();
        services.AddApplication();
        services.AddInfrastructure(config);
        services.AddScoped<ICoreAccountingClient, FakeCore>().AddScoped<IInsurerClient, FakeInsurer>()
            .AddScoped<IInventoryClient, FakeInventory>().AddScoped<IMailClient, FakeMail>();
        Provider = services.BuildServiceProvider();

        using var scope = Provider.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        await db.Database.MigrateAsync();
        var clock = scope.ServiceProvider.GetRequiredService<IClock>();
        var ctx = new SeedContext(db, 2026, clock.Now, clock.Today, new Dictionary<string, string>());
        scope.ServiceProvider.GetRequiredService<AuditInterceptor>().Enabled = false;
        BranchSeed.Run(ctx); CatalogSeed.Run(ctx); PeopleSeed.Run(ctx);
        HerediaId = db.Branches.First(b => b.Name == "Heredia").Id;
    }

    public async Task DisposeAsync()
    {
        await Provider.DisposeAsync();
        await _pg.DisposeAsync();
    }

    private static int _seq;

    /// <summary>Crea un servicio inmediato de paquete Tradicional en Heredia (con capilla opcional).</summary>
    public async Task<ServiceDetailDto> CreateServiceAsync(IServiceScope scope, long? chapelId = null, DateTimeOffset? at = null, string? familyId = null)
    {
        var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        var pkg = await db.Packages.Include(p => p.Items).FirstAsync(p => p.Name == "Tradicional");
        var n = Interlocked.Increment(ref _seq);
        var req = new CreateServiceRequest(ServiceKind.Immediate, HerediaId,
            new DeceasedInput($"Fallecido Prueba {n}", $"1-{1000 + n:D4}-0001", new DateOnly(1950, 1, 1), DateOnly.FromDateTime(DateTime.UtcNow)),
            new FamilyInput($"Familiar Prueba {n}", familyId ?? $"1-{2000 + n:D4}-0002", "Hijo", "8888-1234", $"familia{n}@correo.example.cr", false),
            pkg.Id, pkg.Items.Select(i => new LineInput(i.ComponentId, null, i.Qty)).ToList(),
            chapelId is null ? WakePlace.Home : WakePlace.Chapel, chapelId is null ? "Dirección de prueba" : null, chapelId,
            FinalDestination.Cemetery, "Cementerio General", at ?? DateTimeOffset.UtcNow.AddDays(2), Payer.Family, null, null);
        return await scope.ServiceProvider.GetRequiredService<CreateServiceHandler>().HandleAsync(req, default);
    }
}
