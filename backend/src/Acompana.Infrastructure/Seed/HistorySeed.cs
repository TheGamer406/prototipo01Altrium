/*
 * Archivo:    backend/src/Acompana.Infrastructure/Seed/HistorySeed.cs
 * Qué hace:   12 meses de servicios históricos coherentes para la analítica.
 * Recibe:     Faker con semilla DEMO_SEED (2026) y 'hoy' = fecha del servidor en Costa Rica.
 * Procesa:    ~1 800 servicios, diciembre y enero al doble; ~30 % personalizados, ~35 % cremación, ~25
 *               % aseguradora; historial de estados con tiempos realistas; facturas y pagos.
 * Devuelve:   Servicios
 * Se conecta: ← DemoSeeder / ResetDemoHandler · → AppDbContext
 * Reglas:     Datos ficticios; cédulas formato 1-0234-0567
 * Estado:     Pendiente
 */
