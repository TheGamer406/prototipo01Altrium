/*
 * Archivo:    backend/src/Acompana.Infrastructure/Persistence/DatabaseInitializer.cs
 * Qué hace:   Al arrancar: aplica migraciones y siembra si la BD está vacía.
 * Recibe:     IServiceProvider al inicio de la app.
 * Procesa:    Database.MigrateAsync(); si branches está vacía → IDemoSeeder.SeedAsync(DEMO_SEED) →
 *               refresca analítica.
 * Devuelve:   Task; reintenta 10 veces si la BD aún no acepta conexiones.
 * Se conecta: ← Api/Program.cs · → DemoSeeder, AnalyticsViews
 * Estado:     Completo
 */

using Acompana.Application.Admin;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Acompana.Infrastructure.Persistence;

public static class DatabaseInitializer
{
    public static async Task InitializeAsync(IServiceProvider services, CancellationToken ct = default)
    {
        for (var attempt = 1; ; attempt++)
        {
            try
            {
                using var scope = services.CreateScope();
                var log = scope.ServiceProvider.GetRequiredService<ILoggerFactory>().CreateLogger("DatabaseInitializer");
                var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
                await db.Database.MigrateAsync(ct);
                if (!await db.Branches.AnyAsync(ct))
                {
                    var seed = int.TryParse(scope.ServiceProvider.GetRequiredService<IConfiguration>()["DEMO_SEED"], out var s) ? s : 2026;
                    log.LogInformation("Base vacía: sembrando datos del demo (semilla {Seed})", seed);
                    var summary = await scope.ServiceProvider.GetRequiredService<IDemoSeeder>().SeedAsync(seed, ct);
                    log.LogInformation("Seed listo: {Summary}", summary);
                }
                return;
            }
            catch (Exception) when (attempt < 10)
            {
                await Task.Delay(TimeSpan.FromSeconds(3), ct);
            }
        }
    }
}
