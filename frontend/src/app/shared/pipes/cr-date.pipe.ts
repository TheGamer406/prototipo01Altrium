/**
 * Archivo:    frontend/src/app/shared/pipes/cr-date.pipe.ts
 * Qué hace:   Formato de fecha y hora de Costa Rica.
 * Recibe:     string ISO o Date + formato ('date' | 'datetime' | 'time').
 * Procesa:    Intl.DateTimeFormat('es-CR', { timeZone: 'America/Costa_Rica', hour12: false })
 * Devuelve:   '25/11/2026', '25/11/2026 14:32' o '14:32'.
 * Se conecta: ← todas las vistas con fechas
 * Estado:     Pendiente
 */
