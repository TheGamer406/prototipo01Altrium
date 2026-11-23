/*
 * Archivo:    backend/src/Acompana.Infrastructure/DependencyInjection.cs
 * Qué hace:   Registra EF Core, interceptores, clientes HTTP con Polly, seed y reloj.
 * Recibe:     IServiceCollection + IConfiguration.
 * Procesa:    AddDbContext<AppDbContext>(Npgsql + AuditInterceptor);
 *               AddHttpClient<ICoreAccountingClient, ...> con ResiliencePolicies; AddScoped<IDemoSeeder,
 *               DemoSeeder>; AddSingleton<IClock, CostaRicaClock>.
 * Devuelve:   IServiceCollection.
 * Se conecta: ← Api/Program.cs (AddInfrastructure())
 * Estado:     Pendiente
 */
