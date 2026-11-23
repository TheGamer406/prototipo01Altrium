/*
 * Archivo:    backend/src/Acompana.Application/Providers/ProviderDtos.cs
 * Qué hace:   DTO de proveedores y ofertas.
 * Recibe:     —
 * Procesa:    —
 * Devuelve:   ProviderDto { id, name, type, zone, email, phone, active, wonLast30d }
 *             OfferDto { id, serviceCode, componentName, category, place, scheduledAt, fixedPriceCrc,
 *               status, sentAt, expiresAt }
 *             OpenOpportunityDto { lineId, serviceCode, componentName, offersSent, minutesOpen,
 *               isOverdue }
 *             AcceptOfferResult { won: bool, message ('Adjudicado' | 'Ya fue asignada') }
 * Se conecta: ← ProvidersController, OffersController · → core/models/providers.models.ts
 * Reglas:     RF-14 a RF-17
 * Estado:     Completo
 */

namespace Acompana.Application.Providers;

public record ProviderDto(long Id, string Name, string Type, string Zone, string Email, string Phone, bool Active, int WonLast30d);
public record OfferDto(Guid Id, string ServiceCode, string ComponentName, string Category, string Place, DateTimeOffset ScheduledAt,
    decimal FixedPriceCrc, string Status, DateTimeOffset SentAt, DateTimeOffset ExpiresAt, string? ImagePath);
public record OpenOpportunityDto(Guid LineId, string ServiceCode, string BranchName, string ComponentName, int OffersSent, int MinutesOpen, bool IsOverdue);
public record AcceptOfferResult(bool Won, string Message);
public record ProviderInboxDto(IReadOnlyList<OfferDto> Open, IReadOnlyList<OfferDto> History);
