/*
 * Archivo:    backend/src/Acompana.Infrastructure/Seed/DemoSeeder.cs
 * Qué hace:   Orquesta el seed completo con semilla fija (Bogus, locale es).
 * Recibe:     Faker con semilla DEMO_SEED (2026) y 'hoy' = fecha del servidor en Costa Rica.
 * Procesa:    Llama en orden: Branch → Catalog → People → History → Today; guarda por lotes.
 * Devuelve:   SeedSummary { branches: 28, employees: ~125, services: ~1 800 + 15-20 hoy, ... }
 * Se conecta: ← DemoSeeder / ResetDemoHandler · → AppDbContext
 * Reglas:     Datos ficticios; cédulas formato 1-0234-0567
 * Estado:     Pendiente
 */
