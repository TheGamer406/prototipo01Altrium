/*
 * Archivo:    backend/src/Acompana.Api/Program.cs
 * Qué hace:   Punto de entrada del API: configura servicios, middleware, Swagger, SignalR y health.
 * Recibe:     Variables de entorno (ConnectionStrings__Default, MockBaseUrl, TZ, DEMO_SEED, ...).
 * Procesa:    AddApplication + AddInfrastructure; Swagger en /api/swagger; MapHub /hubs/ops; al
 *               arrancar migra y siembra si la BD está vacía (DatabaseInitializer).
 * Devuelve:   Proceso HTTP en :8080; GET /health → 200 'ok'.
 * Se conecta: → Application/DependencyInjection, Infrastructure/DependencyInjection
 * Estado:     Parcial: controladores, Swagger y SignalR en el resto de la fase 2
 */
using Acompana.Api.Middleware;
using Acompana.Infrastructure;
using Acompana.Infrastructure.Persistence;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddScoped<DemoCurrentUser>();
builder.Services.AddScoped<Acompana.Application.Common.ICurrentUser>(sp => sp.GetRequiredService<DemoCurrentUser>());
builder.Services.AddInfrastructure(builder.Configuration);

var app = builder.Build();

app.UseMiddleware<DemoUserMiddleware>();
app.MapGet("/health", () => Results.Ok("ok"));

await DatabaseInitializer.InitializeAsync(app.Services);

app.Run();
