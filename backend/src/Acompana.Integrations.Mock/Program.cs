/*
 * Archivo:    backend/src/Acompana.Integrations.Mock/Program.cs
 * Qué hace:   Servicio simulado de integraciones externas (core contable, inventario, aseguradoras,
 *               correo).
 * Recibe:     HTTP en :8081 desde el API (red interna, sin puerto publicado).
 * Procesa:    Minimal API; estado en memoria (MockState); latencia aleatoria 300-1200 ms en cada
 *               endpoint.
 * Devuelve:   Respuestas de los endpoints en Endpoints/*; GET /health.
 * Se conecta: ← Infrastructure/Integrations/* · → Endpoints/*
 * Estado:     Parcial: solo /health; endpoints en fase 2
 */

var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();

app.MapGet("/health", () => Results.Ok("ok"));

app.Run();
