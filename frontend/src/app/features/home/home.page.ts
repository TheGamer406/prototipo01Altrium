/**
 * Archivo:    frontend/src/app/features/home/home.page.ts
 * Qué hace:   Inicio: resumen de la sede.
 * Recibe:     Sede y rol actuales.
 * Procesa:    Pide overview de la sede, servicios de hoy, alertas abiertas, capillas y stock bajo; se
 *               refresca con el hub.
 * Devuelve:   KPI + lista de servicios de hoy + alertas + capillas ocupadas + cofres bajo mínimo.
 * Se conecta: → OversightApi, ServicesApi, OpsHubService
 * Reglas:     RF-19 · RF-29
 * Estado:     Pendiente
 */
