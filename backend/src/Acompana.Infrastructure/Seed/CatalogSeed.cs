/*
 * Archivo:    backend/src/Acompana.Infrastructure/Seed/CatalogSeed.cs
 * Qué hace:   Componentes, paquetes, modelos de cofre, stock y parámetros.
 * Recibe:     Faker con semilla DEMO_SEED (2026) y 'hoy' = fecha del servidor en Costa Rica.
 * Procesa:    ~25 componentes (algunos 50 % o 100 % exentos); 4 paquetes (Básico ₡850 000, Tradicional
 *               ₡1 450 000, Premium ₡2 600 000, Cremación ₡1 200 000 = suma de componentes); 10 cofres
 *               (6 nacionales, 4 de España, ₡350 000-₡1 900 000, imagen cofreN.webp); stock con 3-4
 *               sedes en mínimo o cero.
 * Devuelve:   Listas de entidades
 * Se conecta: ← DemoSeeder / ResetDemoHandler · → AppDbContext
 * Reglas:     Datos ficticios; cédulas formato 1-0234-0567
 * Estado:     Pendiente
 */
