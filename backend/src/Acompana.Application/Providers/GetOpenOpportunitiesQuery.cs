/*
 * Archivo:    backend/src/Acompana.Application/Providers/GetOpenOpportunitiesQuery.cs
 * Qué hace:   Vista del encargado: líneas aún sin proveedor.
 * Recibe:     branch? + ICurrentUser.
 * Procesa:    Líneas Offered con minutos abiertos y si superan el plazo.
 * Devuelve:   OpenOpportunityDto[].
 * Se conecta: ← GET /api/providers/opportunities
 * Reglas:     RF-17
 * Estado:     Completo
 */

using Acompana.Application.Common;
using Acompana.Domain.Enums;
using Microsoft.EntityFrameworkCore;

namespace Acompana.Application.Providers;

public sealed class GetOpenOpportunitiesQuery
{
    private readonly IAppDbContext _db;
    private readonly ICurrentUser _user;
    private readonly IClock _clock;
    public GetOpenOpportunitiesQuery(IAppDbContext db, ICurrentUser user, IClock clock) { _db = db; _user = user; _clock = clock; }

    public async Task<IReadOnlyList<OpenOpportunityDto>> HandleAsync(long? branch, CancellationToken ct)
    {
        var timeout = int.TryParse(await _db.Settings.AsNoTracking().Where(s => s.Key == "ProviderAcceptTimeoutMin").Select(s => s.Value).FirstOrDefaultAsync(ct), out var t) ? t : 30;
        var rows = await (from l in _db.ServiceLines.AsNoTracking()
                          join s in _db.Services.AsNoTracking().ForUser(_user, branch) on l.ServiceId equals s.Id
                          join c in _db.Components.AsNoTracking() on l.ComponentId equals c.Id
                          join b in _db.Branches.AsNoTracking() on s.BranchId equals b.Id
                          where l.Status == LineStatus.Offered && l.ProviderId == null
                          select new { l.Id, s.Code, Branch = b.Name, Comp = c.Name,
                              Sent = _db.ProviderOffers.Count(o => o.ServiceLineId == l.Id),
                              First = _db.ProviderOffers.Where(o => o.ServiceLineId == l.Id).Min(o => (DateTimeOffset?)o.SentAt) }).ToListAsync(ct);
        var now = _clock.Now;
        return rows.Select(r =>
        {
            var minutes = r.First is null ? 0 : (int)(now - r.First.Value).TotalMinutes;
            return new OpenOpportunityDto(r.Id, r.Code, r.Branch, r.Comp, r.Sent, minutes, minutes > timeout);
        }).OrderByDescending(o => o.MinutesOpen).ToList();
    }
}
