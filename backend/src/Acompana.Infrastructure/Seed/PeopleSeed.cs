/*
 * Archivo:    backend/src/Acompana.Infrastructure/Seed/PeopleSeed.cs
 * Qué hace:   Personal, proveedores y aseguradoras.
 * Recibe:     Faker con semilla DEMO_SEED (2026) y 'hoy' = fecha del servidor en Costa Rica.
 * Procesa:    ~125 empleados (GAM 2-3 + encargado; fuera 1 que hace todo; 6 operadores; 5 gerencia);
 *               30 proveedores (12 flores, 10 catering, 8 música) por zona; 3 aseguradoras.
 * Devuelve:   Listas
 * Se conecta: ← DemoSeeder / ResetDemoHandler · → AppDbContext
 * Reglas:     Datos ficticios; cédulas formato 1-0234-0567
 * Estado:     Pendiente
 */
