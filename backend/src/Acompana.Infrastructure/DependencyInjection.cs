/*
 * Archivo:    backend/src/Acompana.Infrastructure/DependencyInjection.cs
 * Qué hace:   Registra EF Core, interceptores, clientes HTTP con Polly, seed y reloj.
 * Recibe:     IServiceCollection + IConfiguration.
 * Procesa:    AddDbContext<AppDbContext>(Npgsql + AuditInterceptor);
 *               AddHttpClient<ICoreAccountingClient, ...> con ResiliencePolicies; AddScoped<IDemoSeeder,
 *               DemoSeeder>; AddSingleton<IClock, CostaRicaClock>.
 * Devuelve:   IServiceCollection.
 * Se conecta: ← Api/Program.cs (AddInfrastructure())
 * Estado:     Completo
 */

using Acompana.Application.Admin;
using Acompana.Application.Common;
using Acompana.Infrastructure.Persistence;
using Acompana.Infrastructure.Seed;
using Acompana.Infrastructure.Time;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Acompana.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration config)
    {
        services.AddSingleton<IClock, CostaRicaClock>();
        services.AddScoped<AuditInterceptor>();
        services.AddDbContext<AppDbContext>((sp, o) => o
            .UseNpgsql(config.GetConnectionString("Default"))
            .UseSnakeCaseNamingConvention()
            .AddInterceptors(sp.GetRequiredService<AuditInterceptor>()));
        services.AddScoped<IAppDbContext>(sp => sp.GetRequiredService<AppDbContext>());
        services.AddScoped<IDemoSeeder, DemoSeeder>();
        return services;
    }
}
