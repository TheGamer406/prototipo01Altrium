/*
 * Archivo:    backend/src/Acompana.Infrastructure/Seed/TodaySeed.cs
 * Qué hace:   Servicios de hoy y los casos del guion.
 * Recibe:     Faker con semilla DEMO_SEED (2026) y 'hoy' = fecha del servidor en Costa Rica.
 * Procesa:    15-20 servicios: 2 anticipados, 3 sin acta, 2 en facturación (1 rechazado), 4 pagados
 *               con ofertas abiertas, 6 en curso, 2 finalizados; 1 línea urgente (< 2 h); 1 oferta
 *               vencida con alerta; caso de las flores en audit_log; correos simulados; traslados (1 sin
 *               señal sincronizado, 1 con demora); 6 cuerpos en la cola del crematorio.
 * Devuelve:   Servicios del día
 * Se conecta: ← DemoSeeder / ResetDemoHandler · → AppDbContext
 * Reglas:     Datos ficticios; cédulas formato 1-0234-0567
 * Estado:     Pendiente
 */
