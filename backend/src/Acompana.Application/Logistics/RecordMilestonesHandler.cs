/*
 * Archivo:    backend/src/Acompana.Application/Logistics/RecordMilestonesHandler.cs
 * Qué hace:   Recibe un lote de hitos del teléfono del chofer (incluye los marcados sin señal).
 * Recibe:     transferId + MilestoneBatch.
 * Procesa:    INSERT … ON CONFLICT (client_uuid) DO NOTHING por hito; SyncedAt = now; Source según el
 *               teléfono.
 * Devuelve:   { accepted: n, duplicates: m }.
 * Se conecta: ← POST /api/transfers/{id}/milestones · → TransferDelayChecker, IOpsNotifier
 * Reglas:     RF-22 · RNF-05
 * Estado:     Pendiente
 */
