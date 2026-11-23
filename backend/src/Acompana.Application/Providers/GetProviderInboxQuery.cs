/*
 * Archivo:    backend/src/Acompana.Application/Providers/GetProviderInboxQuery.cs
 * Qué hace:   Bandeja del proveedor: ofertas abiertas e historial.
 * Recibe:     providerId (del rol Proveedor o query string en modo admin).
 * Procesa:    Ofertas del proveedor; abiertas primero, ordenadas por vencimiento.
 * Devuelve:   { open: OfferDto[], history: OfferDto[] }.
 * Se conecta: ← GET /api/offers?providerId=
 * Reglas:     RF-15 · RF-16
 * Estado:     Completo
 */

using Acompana.Application.Common;
using Acompana.Domain.Enums;
using Microsoft.EntityFrameworkCore;

namespace Acompana.Application.Providers;

public sealed class GetProviderInboxQuery
{
    private readonly IAppDbContext _db;
    private readonly ICurrentUser _user;
    public GetProviderInboxQuery(IAppDbContext db, ICurrentUser user) { _db = db; _user = user; }

    public async Task<ProviderInboxDto> HandleAsync(long? providerId, CancellationToken ct)
    {
        var pid = _user.Role == DemoRole.Provider ? _user.ProviderId ?? providerId : providerId;
        if (pid is null) throw new ValidationException("providerId", "Seleccione el proveedor.");

        var rows = await (from o in _db.ProviderOffers.AsNoTracking()
                          join l in _db.ServiceLines.AsNoTracking() on o.ServiceLineId equals l.Id
                          join c in _db.Components.AsNoTracking() on l.ComponentId equals c.Id
                          join s in _db.Services.AsNoTracking() on l.ServiceId equals s.Id
                          join b in _db.Branches.AsNoTracking() on s.BranchId equals b.Id
                          where o.ProviderId == pid
                          orderby o.SentAt descending
                          select new { o.Id, s.Code, Comp = c.Name, c.Category, c.ImagePath, s.WakePlace, s.WakeAddress, Branch = b.Name, s.ScheduledAt, Price = l.UnitPriceCrc, o.Status, o.SentAt, l.DueAt }).Take(200).ToListAsync(ct);

        OfferDto Map(dynamic r) => new(r.Id, r.Code, r.Comp, r.Category.ToString(),
            r.WakePlace == WakePlace.Chapel ? $"Capilla de la sede {r.Branch}" : r.WakePlace == WakePlace.Church ? "Iglesia indicada por la familia" : (r.WakeAddress ?? "Domicilio"),
            r.ScheduledAt, r.Price, r.Status.ToString(), r.SentAt, r.DueAt ?? r.ScheduledAt.AddHours(-2), r.ImagePath);

        var open = rows.Where(r => r.Status == OfferStatus.Open).OrderBy(r => r.DueAt).Select(r => Map(r)).ToList();
        var history = rows.Where(r => r.Status != OfferStatus.Open).Select(r => Map(r)).ToList();
        return new ProviderInboxDto(open, history);
    }
}
