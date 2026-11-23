/*
 * Archivo:    backend/src/Acompana.Infrastructure/Persistence/DatabaseInitializer.cs
 * Qué hace:   Al arrancar: aplica migraciones y siembra si la BD está vacía.
 * Recibe:     IServiceProvider al inicio de la app.
 * Procesa:    Database.MigrateAsync(); si branches está vacía → IDemoSeeder.SeedAsync(DEMO_SEED) →
 *               refresca analítica.
 * Devuelve:   Task; reintenta 10 veces si la BD aún no acepta conexiones.
 * Se conecta: ← Api/Program.cs · → DemoSeeder, AnalyticsViews
 * Estado:     Pendiente
 */
