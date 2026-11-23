/*
 * Archivo:    backend/src/Acompana.Application/Oversight/OversightDtos.cs
 * Qué hace:   DTO de vista central, bitácora, alertas y mensajes.
 * Recibe:     —
 * Procesa:    —
 * Devuelve:   OverviewDto { kpis{ongoing, chapelsBusy, lowStock, openAlerts}, branches[{id, code,
 *               name, zone, lat, lng, ongoing, chapelsBusy, chapelsTotal, lowStock}] }
 *             AuditEntryDto { at, actorRole, actorName, branchName, entity, entityId, action,
 *               oldValues, newValues }
 *             AlertDto { id, type, branchName, serviceCode?, message, createdAt, acknowledgedAt? }
 *             MessageDto { id, toAddress, kind, subject, body, serviceCode?, createdAt }
 * Se conecta: ← OversightController · → core/models/oversight.models.ts
 * Reglas:     RF-29 · RF-31
 * Estado:     Pendiente
 */
