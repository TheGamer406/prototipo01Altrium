/**
 * Archivo:    frontend/src/app/features/driver-app/offline-queue.service.ts
 * Qué hace:   Cola de hitos sin señal.
 * Recibe:     Hito con clientUuid (crypto.randomUUID()).
 * Procesa:    Guarda en localStorage; al volver la señal envía el lote; reintenta hasta 200.
 * Devuelve:   Signal con pendientes.
 * Se conecta: ← driver-app.page · → LogisticsApi.sendMilestones
 * Reglas:     RF-22 · RNF-05
 * Estado:     Pendiente
 */
