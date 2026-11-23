/*
 * Archivo:    backend/src/Acompana.Api/Program.cs
 * Qué hace:   Punto de entrada del API: configura servicios, middleware, Swagger, SignalR y health.
 * Recibe:     Variables de entorno (ConnectionStrings__Default, MockBaseUrl, TZ, DEMO_SEED, ...).
 * Procesa:    AddApplication + AddInfrastructure; Swagger en /api/swagger; MapHub /hubs/ops; al
 *               arrancar migra y siembra si la BD está vacía (DatabaseInitializer).
 * Devuelve:   Proceso HTTP en :8080; GET /health → 200 'ok'.
 * Se conecta: → Application/DependencyInjection, Infrastructure/DependencyInjection
 * Estado:     Parcial: solo /health; lo demás en fase 2
 */

var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();

app.MapGet("/health", () => Results.Ok("ok"));

app.Run();
