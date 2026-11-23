/*
 * Archivo:    backend/src/Acompana.Infrastructure/Seed/DemoSeeder.cs
 * Qué hace:   Orquesta el seed completo con semilla fija (Bogus, locale es).
 * Recibe:     Faker con semilla DEMO_SEED (2026) y 'hoy' = fecha del servidor en Costa Rica.
 * Procesa:    Llama en orden: Branch → Catalog → People → History → Today; guarda por lotes.
 * Devuelve:   SeedSummary { branches: 28, employees: ~125, services: ~1 800 + 15-20 hoy, ... }
 * Se conecta: ← DemoSeeder / ResetDemoHandler · → AppDbContext
 * Reglas:     Datos ficticios; cédulas formato 1-0234-0567
 * Estado:     Completo
 */

using Acompana.Application.Admin;
using Acompana.Application.Common;
using Acompana.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace Acompana.Infrastructure.Seed;

public sealed class DemoSeeder : IDemoSeeder
{
    private readonly AppDbContext _db;
    private readonly AuditInterceptor _audit;
    private readonly IClock _clock;
    private readonly IConfiguration _config;

    public DemoSeeder(AppDbContext db, AuditInterceptor audit, IClock clock, IConfiguration config)
    {
        _db = db; _audit = audit; _clock = clock; _config = config;
    }

    public async Task<SeedSummary> SeedAsync(int seed, CancellationToken ct)
    {
        var settings = new Dictionary<string, string>
        {
            ["ProviderAcceptTimeoutMin"] = _config["PROVIDER_ACCEPT_TIMEOUT_MIN"] ?? "30",
            ["TransferThresholdMin"] = _config["TRANSFER_THRESHOLD_MIN"] ?? "20",
            ["DefaultMinStock"] = "2",
        };
        var ctx = new SeedContext(_db, seed, _clock.Now, _clock.Today, settings);
        var wasEnabled = _audit.Enabled;
        _audit.Enabled = false;
        _db.ChangeTracker.AutoDetectChangesEnabled = true;
        try
        {
            await Task.Run(() =>
            {
                BranchSeed.Run(ctx);
                CatalogSeed.Run(ctx);
                PeopleSeed.Run(ctx);
                HistorySeed.Run(ctx);
                TodaySeed.Run(ctx);
            }, ct);
            await _db.Database.ExecuteSqlRawAsync(AnalyticsViews.RefreshAllSql, ct);
            return new SeedSummary(
                await _db.Branches.CountAsync(ct), await _db.Employees.CountAsync(ct), await _db.Services.CountAsync(ct),
                await _db.Providers.CountAsync(ct), await _db.Components.CountAsync(ct), await _db.Packages.CountAsync(ct));
        }
        finally
        {
            _audit.Enabled = wasEnabled;
            _db.ChangeTracker.Clear();
        }
    }
}
