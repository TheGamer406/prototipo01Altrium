/*
 * Archivo:    backend/src/Acompana.Application/Services/GetServicesQuery.cs
 * Qué hace:   Lista paginada de servicios con filtros.
 * Recibe:     status?, branch?, q? (código, nombre del fallecido o cédula), page, pageSize (25), sort.
 * Procesa:    BranchScope.ForUser; búsqueda ILIKE; orden por ScheduledAt desc por defecto; calcula
 *               hasUrgentLine.
 * Devuelve:   PagedResult<ServiceListItemDto>.
 * Se conecta: ← GET /api/services · también lo usa la búsqueda global de la barra superior
 * Reglas:     RF-19 · RF-30
 * Estado:     Completo
 */

using Acompana.Application.Common;
using Acompana.Domain.Enums;
using Microsoft.EntityFrameworkCore;

namespace Acompana.Application.Services;

public sealed class GetServicesQuery
{
    private readonly IAppDbContext _db;
    private readonly ICurrentUser _user;
    private readonly IClock _clock;
    public GetServicesQuery(IAppDbContext db, ICurrentUser user, IClock clock) { _db = db; _user = user; _clock = clock; }

    public async Task<PagedResult<ServiceListItemDto>> HandleAsync(string? status, long? branch, string? q, int page, int pageSize, string? sort, CancellationToken ct)
    {
        page = Math.Max(1, page); pageSize = pageSize is < 1 or > 100 ? 25 : pageSize;
        var query = _db.Services.AsNoTracking().ForUser(_user, branch);
        if (!string.IsNullOrWhiteSpace(status) && Enum.TryParse<ServiceStatus>(status, true, out var st)) query = query.Where(s => s.Status == st);
        if (!string.IsNullOrWhiteSpace(q))
        {
            var t = q.Trim().ToLower();
            query = query.Where(s => s.Code.ToLower().Contains(t) || s.Deceased!.FullName.ToLower().Contains(t) || s.Deceased!.NationalId.Contains(t));
        }
        query = (sort ?? "scheduledAt desc") switch
        {
            "scheduledAt asc" => query.OrderBy(s => s.ScheduledAt),
            "code asc" => query.OrderBy(s => s.Code),
            "code desc" => query.OrderByDescending(s => s.Code),
            "total asc" => query.OrderBy(s => s.TotalCrc),
            "total desc" => query.OrderByDescending(s => s.TotalCrc),
            _ => query.OrderByDescending(s => s.ScheduledAt),
        };
        var total = await query.CountAsync(ct);
        var limit = _clock.Now.AddHours(2);
        var items = await query.Skip((page - 1) * pageSize).Take(pageSize).Select(s => new
        {
            s.Id, s.Code, Deceased = s.Deceased!.FullName, Branch = s.Branch!.Name, s.Status, s.Kind, s.ScheduledAt, s.TotalCrc, s.IsCustomPackage,
            Urgent = s.Lines.Any(l => (l.Status == LineStatus.Pending || l.Status == LineStatus.Offered) && l.DueAt != null && l.DueAt < limit),
        }).ToListAsync(ct);
        return new PagedResult<ServiceListItemDto>(items.Select(i => new ServiceListItemDto(i.Id, i.Code, i.Deceased, i.Branch, i.Status.ToString(),
            i.Kind.ToString(), i.ScheduledAt, i.TotalCrc, i.IsCustomPackage, i.Urgent)).ToList(), page, pageSize, total);
    }
}
