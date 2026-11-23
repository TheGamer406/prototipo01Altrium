/*
 * Archivo:    backend/src/Acompana.Domain/Services/ServiceLine.cs
 * Qué hace:   Entidad ServiceLine, se guarda en la tabla `service_lines`.
 * Recibe:     Campos: ServiceId, ComponentId, CasketModelId?, Qty, UnitPriceCrc, TaxExemptPct,
 *               LineTotalCrc, Status (LineStatus), ProviderId?, AcceptedPriceCrc?, AcceptedAt?, DueAt?,
 *               Version. Los llena el handler que la crea o EF Core al leer.
 * Procesa:    Precio y exoneración se copian del catálogo al crear la línea (el catálogo puede cambiar
 *               después).
 *             IsUrgent(now) = Status ∈ {Pending, Offered} y DueAt − now < 2 h (RF-19).
 *             La adjudicación NO se hace aquí: es un UPDATE condicional en AcceptOfferHandler
 *               (RNF-16).
 * Devuelve:   Nada; es estado. Hereda Id, CreatedAt y UpdatedAt de Entity.
 * Se conecta: ← Service.Lines · → ProviderOffer, tablero del servicio
 * Reglas:     RF-04 · RF-05 · RF-10 · RF-16 · RF-19
 * Estado:     Completo
 */

using Acompana.Domain.Catalog;
using Acompana.Domain.Common;
using Acompana.Domain.Enums;

namespace Acompana.Domain.Services;

public class ServiceLine : GuidEntity
{
    public Guid ServiceId { get; set; }
    public long ComponentId { get; set; }
    public Component? Component { get; set; }
    public long? CasketModelId { get; set; }
    public CasketModel? CasketModel { get; set; }
    public int Qty { get; set; } = 1;
    public decimal UnitPriceCrc { get; set; }
    public int TaxExemptPct { get; set; }
    public decimal LineTotalCrc { get; set; }
    public LineStatus Status { get; set; } = LineStatus.Pending;
    public long? ProviderId { get; set; }
    public decimal? AcceptedPriceCrc { get; set; }
    public DateTimeOffset? AcceptedAt { get; set; }
    public DateTimeOffset? DueAt { get; set; }
    public uint Version { get; set; }

    public bool IsUrgent(DateTimeOffset now) =>
        Status is LineStatus.Pending or LineStatus.Offered && DueAt is not null && DueAt.Value - now < TimeSpan.FromHours(2);
}
