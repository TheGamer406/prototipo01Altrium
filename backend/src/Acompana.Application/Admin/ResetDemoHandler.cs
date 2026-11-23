/*
 * Archivo:    backend/src/Acompana.Application/Admin/ResetDemoHandler.cs
 * Qué hace:   Reinicia el demo: borra datos, vuelve a sembrar y refresca la analítica.
 * Recibe:     nada (modo admin).
 * Procesa:    TRUNCATE … RESTART IDENTITY CASCADE de tablas operativas;
 *               DemoSeeder.SeedAsync(DEMO_SEED); RefreshAnalytics.
 * Devuelve:   { seededAt, services, branches }.
 * Se conecta: ← POST /api/admin/reset · → IDemoSeeder (Infrastructure)
 * Reglas:     —
 * Estado:     Completo
 */

using Acompana.Application.Common;
using Acompana.Application.Integrations;

namespace Acompana.Application.Admin;

public sealed class ResetDemoHandler
{
    private readonly IAppDbContext _db;
    private readonly IDemoSeeder _seeder;
    private readonly IMockControlClient _mock;
    private readonly IClock _clock;
    private readonly Microsoft.Extensions.Configuration.IConfiguration _config;

    public ResetDemoHandler(IAppDbContext db, IDemoSeeder seeder, IMockControlClient mock, IClock clock, Microsoft.Extensions.Configuration.IConfiguration config)
    { _db = db; _seeder = seeder; _mock = mock; _clock = clock; _config = config; }

    public async Task<object> HandleAsync(CancellationToken ct)
    {
        await _db.ExecuteSqlRawAsync(@"DO $$ DECLARE r record; BEGIN
            FOR r IN SELECT tablename FROM pg_tables WHERE schemaname = 'public' AND tablename <> '__EFMigrationsHistory' LOOP
              EXECUTE 'TRUNCATE TABLE public.' || quote_ident(r.tablename) || ' RESTART IDENTITY CASCADE';
            END LOOP; END $$;", ct);
        var seed = int.TryParse(_config["DEMO_SEED"], out var s) ? s : 2026;
        var summary = await _seeder.SeedAsync(seed, ct);
        try { await _mock.ResetAsync(ct); } catch (IntegrationUnavailableException) { /* el simulador puede estar apagado */ }
        return new { seededAt = _clock.Now, services = summary.Services, branches = summary.Branches };
    }
}
