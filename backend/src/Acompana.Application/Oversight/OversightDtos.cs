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
 * Estado:     Completo
 */

namespace Acompana.Application.Oversight;

public record OverviewKpisDto(int Ongoing, int ChapelsBusy, int LowStock, int OpenAlerts);
public record BranchOverviewDto(long Id, string Code, string Name, string Zone, double Lat, double Lng, int Ongoing, int ChapelsBusy, int ChapelsTotal, int LowStock);
public record OverviewDto(OverviewKpisDto Kpis, IReadOnlyList<BranchOverviewDto> Branches);
public record AuditEntryDto(long Id, DateTimeOffset At, string ActorRole, string ActorName, string? BranchName, string Entity, string EntityId, string Action, string OldValues, string NewValues);
public record AlertDto(long Id, string Type, string BranchName, string? ServiceCode, string Message, DateTimeOffset CreatedAt, DateTimeOffset? AcknowledgedAt);
public record MessageDto(long Id, string ToAddress, string Kind, string Subject, string Body, string? ServiceCode, DateTimeOffset CreatedAt);
