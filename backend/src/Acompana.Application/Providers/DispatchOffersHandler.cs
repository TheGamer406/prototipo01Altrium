/*
 * Archivo:    backend/src/Acompana.Application/Providers/DispatchOffersHandler.cs
 * Qué hace:   Al pagar, envía ofertas a todos los proveedores activos del tipo y zona de cada línea.
 * Recibe:     serviceId (lo llama RegisterPaymentHandler).
 * Procesa:    por cada línea con RequiresProvider: proveedores activos con Type = ProviderType y Zone
 *               = zona de la sede
 *             crea ProviderOffer(Open) por proveedor; línea → Offered; DueAt = ScheduledAt − 2 h
 *             correo ProviderOffer con lugar, hora y precio fijo
 * Devuelve:   int ofertas creadas.
 * Se conecta: ← RegisterPaymentHandler · → IMailClient, IOpsNotifier (bandeja del proveedor se
 *               actualiza)
 * Reglas:     RF-15
 * Estado:     Completo
 *
 * Contrato:
 *   Task<int> HandleAsync(Guid serviceId, CancellationToken ct)
 *     Recibe:     servicio pagado
 *     Procesa:    ver arriba
 *     Devuelve:   cantidad de ofertas
 */

using Acompana.Application.Common;
using Acompana.Domain.Enums;
using Acompana.Domain.Oversight;
using Acompana.Domain.Providers;
using Microsoft.EntityFrameworkCore;

namespace Acompana.Application.Providers;

public sealed class DispatchOffersHandler
{
    private readonly IAppDbContext _db;
    private readonly IClock _clock;
    private readonly IOpsNotifier _notifier;
    public DispatchOffersHandler(IAppDbContext db, IClock clock, IOpsNotifier notifier) { _db = db; _clock = clock; _notifier = notifier; }

    /// <summary>Crea las ofertas y los correos a proveedores (registro directo en outbound_messages) y guarda.</summary>
    public async Task<int> HandleAsync(Guid serviceId, CancellationToken ct)
    {
        var s = await _db.Services.Include(x => x.Branch).Include(x => x.Lines).ThenInclude(l => l.Component)
            .FirstAsync(x => x.Id == serviceId, ct);
        var now = _clock.Now;
        var zone = s.Branch!.Zone;
        var place = s.WakePlace switch
        {
            WakePlace.Chapel => $"Capilla de la sede {s.Branch.Name}",
            WakePlace.Church => "Iglesia indicada por la familia",
            _ => s.WakeAddress ?? "Domicilio de la familia",
        };
        var count = 0;
        foreach (var line in s.Lines.Where(l => l.Component!.RequiresProvider && l.Status == LineStatus.Pending))
        {
            var pool = await _db.Providers.Where(p => p.Active && p.Type == line.Component!.ProviderType && p.Zone == zone).ToListAsync(ct);
            line.DueAt ??= s.ScheduledAt.AddHours(-2);
            if (pool.Count == 0)
            {
                _db.Alerts.Add(new Alert { Type = AlertType.ProviderTimeout, BranchId = s.BranchId, ServiceId = s.Id,
                    Message = $"No hay proveedores activos de {line.Component!.Name} en la zona de {s.Branch.Name} para {s.Code}." });
                continue;
            }
            foreach (var p in pool)
            {
                _db.ProviderOffers.Add(new ProviderOffer { ServiceLineId = line.Id, ProviderId = p.Id, SentAt = now, Status = OfferStatus.Open });
                _db.OutboundMessages.Add(new OutboundMessage
                {
                    ToAddress = p.Email, Kind = MessageKind.ProviderOffer, ServiceId = s.Id,
                    Subject = $"Nueva oferta: {line.Component!.Name}",
                    Body = $"Lugar: {place}. Fecha: {s.ScheduledAt.ToOffset(TimeSpan.FromHours(-6)):dd/MM/yyyy HH:mm}. Precio fijo: ₡{line.UnitPriceCrc:N0}. Acepte con un clic en su bandeja.",
                });
                count++;
            }
            line.Status = LineStatus.Offered;
        }
        await _db.SaveChangesAsync(ct);
        return count;
    }
}
