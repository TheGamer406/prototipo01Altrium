/*
 * Archivo:    backend/src/Acompana.Api/Program.cs
 * Qué hace:   Punto de entrada del API: configura servicios, middleware, Swagger, SignalR y health.
 * Recibe:     Variables de entorno (ConnectionStrings__Default, MockBaseUrl, TZ, DEMO_SEED, ...).
 * Procesa:    AddApplication + AddInfrastructure; Swagger en /api/swagger; MapHub /hubs/ops; al
 *               arrancar migra y siembra si la BD está vacía (DatabaseInitializer).
 * Devuelve:   Proceso HTTP en :8080; GET /health → 200 'ok'.
 * Se conecta: → Application/DependencyInjection, Infrastructure/DependencyInjection
 * Estado:     Completo
 */
using System.Text.Json;
using System.Text.Json.Serialization;
using Acompana.Api.Hubs;
using Acompana.Api.Middleware;
using Acompana.Api.Realtime;
using Acompana.Api.Workers;
using Acompana.Application;
using Acompana.Application.Common;
using Acompana.Infrastructure;
using Acompana.Infrastructure.Persistence;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers().AddJsonOptions(o =>
{
    o.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
    o.JsonSerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
});
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new() { Title = "Acompaña API", Version = "v1", Description = "Prototipo ATRIUM Solutions. Rol y sede por X-Demo-Role y X-Demo-Branch." });
    c.AddSecurityDefinition("X-Demo-Role", new() { Type = Microsoft.OpenApi.Models.SecuritySchemeType.ApiKey, In = Microsoft.OpenApi.Models.ParameterLocation.Header, Name = "X-Demo-Role" });
    c.CustomSchemaIds(t => t.FullName!.Replace('+', '.'));
});
builder.Services.AddSignalR().AddJsonProtocol(o => o.PayloadSerializerOptions.Converters.Add(new JsonStringEnumConverter()));
builder.Services.AddScoped<DemoCurrentUser>();
builder.Services.AddScoped<ICurrentUser>(sp => sp.GetRequiredService<DemoCurrentUser>());
builder.Services.AddSingleton<IOpsNotifier, SignalROpsNotifier>();
builder.Services.AddApplication();
builder.Services.AddInfrastructure(builder.Configuration);
builder.Services.AddHostedService<OpsMonitorWorker>();

var app = builder.Build();

app.UseMiddleware<ErrorHandlingMiddleware>();
app.UseSwagger(c => c.RouteTemplate = "api/swagger/{documentName}/swagger.json");
app.UseSwaggerUI(c => { c.RoutePrefix = "api/swagger"; c.SwaggerEndpoint("/api/swagger/v1/swagger.json", "Acompaña API v1"); });
app.UseMiddleware<DemoUserMiddleware>();
app.MapGet("/health", () => Results.Ok("ok"));
app.MapControllers();
app.MapHub<OpsHub>("/hubs/ops");

await DatabaseInitializer.InitializeAsync(app.Services);

app.Run();
