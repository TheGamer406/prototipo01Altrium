/*
 * Archivo:    backend/src/Acompana.Application/Services/GetServiceBoardQuery.cs
 * Qué hace:   Tablero del servicio: estado y responsable de cada componente.
 * Recibe:     id.
 * Procesa:    Líneas + proveedor adjudicado + IsUrgent(now) (< 2 h para DueAt y aún sin aceptar).
 * Devuelve:   ServiceBoardDto.
 * Se conecta: ← GET /api/services/{id}/board · se refresca con ServiceUpdated/OfferAwarded
 * Reglas:     RF-17 · RF-19
 * Estado:     Completo
 */

using Acompana.Application.Common;
using Microsoft.EntityFrameworkCore;

namespace Acompana.Application.Services;

public sealed class GetServiceBoardQuery
{
    private readonly IAppDbContext _db;
    private readonly ICurrentUser _user;
    private readonly IClock _clock;
    public GetServiceBoardQuery(IAppDbContext db, ICurrentUser user, IClock clock) { _db = db; _user = user; _clock = clock; }

    public async Task<ServiceBoardDto> HandleAsync(Guid id, CancellationToken ct)
    {
        var s = await _db.Services.AsNoTracking().Include(x => x.Lines).ThenInclude(l => l.Component)
            .Include(x => x.Lines).ThenInclude(l => l.CasketModel).FirstOrDefaultAsync(x => x.Id == id, ct);
        if (s is null || (!_user.SeesAllBranches && s.BranchId != _user.BranchId)) throw new NotFoundException("El servicio no existe.");
        var ids = s.Lines.Where(l => l.ProviderId != null).Select(l => l.ProviderId!.Value).Distinct().ToList();
        var providers = await _db.Providers.AsNoTracking().Where(p => ids.Contains(p.Id)).ToDictionaryAsync(p => p.Id, p => p.Name, ct);
        var now = _clock.Now;
        return new ServiceBoardDto(s.Id, s.Code, s.Lines.OrderBy(l => l.Component!.Category).ThenBy(l => l.Component!.Name)
            .Select(l => GetServiceDetailQuery.MapLine(l, providers, now)).ToList());
    }
}
