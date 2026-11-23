/*
 * Archivo:    backend/src/Acompana.Application/Admin/ResetDemoHandler.cs
 * Qué hace:   Reinicia el demo: borra datos, vuelve a sembrar y refresca la analítica.
 * Recibe:     nada (modo admin).
 * Procesa:    TRUNCATE … RESTART IDENTITY CASCADE de tablas operativas;
 *               DemoSeeder.SeedAsync(DEMO_SEED); RefreshAnalytics.
 * Devuelve:   { seededAt, services, branches }.
 * Se conecta: ← POST /api/admin/reset · → IDemoSeeder (Infrastructure)
 * Reglas:     —
 * Estado:     Pendiente
 */
