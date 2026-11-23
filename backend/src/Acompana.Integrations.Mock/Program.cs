/*
 * Archivo:    backend/src/Acompana.Integrations.Mock/Program.cs
 * Qué hace:   Servicio simulado de integraciones externas (core contable, inventario, aseguradoras,
 *               correo).
 * Recibe:     HTTP en :8081 desde el API (red interna, sin puerto publicado).
 * Procesa:    Minimal API; estado en memoria (MockState); latencia aleatoria 300-1200 ms en cada
 *               endpoint.
 * Devuelve:   Respuestas de los endpoints en Endpoints/*; GET /health.
 * Se conecta: ← Infrastructure/Integrations/* · → Endpoints/*
 * Estado:     Completo
 */
using Acompana.Integrations.Mock;
using Acompana.Integrations.Mock.Endpoints;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddSingleton<MockState>();
builder.Services.ConfigureHttpJsonOptions(o => o.SerializerOptions.PropertyNameCaseInsensitive = true);

var app = builder.Build();
app.MapGet("/health", () => Results.Ok("ok"));
CoreAccountingEndpoints.Map(app);
InsurerEndpoints.Map(app);
InventoryEndpoints.Map(app);
MailEndpoints.Map(app);
ControlEndpoints.Map(app);
app.Run();
