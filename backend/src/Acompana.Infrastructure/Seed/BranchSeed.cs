/*
 * Archivo:    backend/src/Acompana.Infrastructure/Seed/BranchSeed.cs
 * Qué hace:   Sedes, capillas, carrozas y crematorio.
 * Recibe:     Faker con semilla DEMO_SEED (2026) y 'hoy' = fecha del servidor en Costa Rica.
 * Procesa:    28 sedes (12 GAM, 16 fuera) con coordenadas aproximadas del cantón; 2-3 capillas GAM,
 *               0-1 fuera; ~35 carrozas; crematorio en San José.
 * Devuelve:   Listas de entidades
 * Se conecta: ← DemoSeeder / ResetDemoHandler · → AppDbContext
 * Reglas:     Datos ficticios; cédulas formato 1-0234-0567
 * Estado:     Pendiente
 */
