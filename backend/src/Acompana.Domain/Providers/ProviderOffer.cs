/*
 * Archivo:    backend/src/Acompana.Domain/Providers/ProviderOffer.cs
 * Qué hace:   Entidad ProviderOffer, se guarda en la tabla `provider_offers`.
 * Recibe:     Campos: ServiceLineId, ProviderId, SentAt, Status (OfferStatus), RespondedAt?. Los llena
 *               el handler que la crea o EF Core al leer.
 * Procesa:    Open → Won | Lost | Expired. Solo una oferta Won por línea.
 * Devuelve:   Nada; es estado. Hereda Id, CreatedAt y UpdatedAt de Entity.
 * Se conecta: ← DispatchOffersHandler, AcceptOfferHandler, OfferTimeoutChecker
 * Reglas:     RF-15 · RF-16 · RF-17
 * Estado:     Completo
 */

using Acompana.Domain.Common;
using Acompana.Domain.Enums;
using Acompana.Domain.Services;

namespace Acompana.Domain.Providers;

public class ProviderOffer : GuidEntity
{
    public Guid ServiceLineId { get; set; }
    public ServiceLine? ServiceLine { get; set; }
    public long ProviderId { get; set; }
    public Provider? Provider { get; set; }
    public DateTimeOffset SentAt { get; set; }
    public OfferStatus Status { get; set; } = OfferStatus.Open;
    public DateTimeOffset? RespondedAt { get; set; }
}
