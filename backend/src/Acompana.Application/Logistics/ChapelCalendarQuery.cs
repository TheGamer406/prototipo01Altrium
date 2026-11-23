/*
 * Archivo:    backend/src/Acompana.Application/Logistics/ChapelCalendarQuery.cs
 * Qué hace:   Calendario de capillas de una sede.
 * Recibe:     branch, from, to.
 * Procesa:    Capillas de la sede con reservas que intersectan el rango.
 * Devuelve:   ChapelCalendarDto.
 * Se conecta: ← GET /api/chapels/calendar · paso 5 del asistente (disponibilidad)
 * Reglas:     RF-20
 * Estado:     Completo
 */

using Acompana.Application.Common;
using Microsoft.EntityFrameworkCore;

namespace Acompana.Application.Logistics;

public sealed class ChapelCalendarQuery
{
    private readonly IAppDbContext _db;
    private readonly ICurrentUser _user;
    public ChapelCalendarQuery(IAppDbContext db, ICurrentUser user) { _db = db; _user = user; }

    public async Task<ChapelCalendarDto> HandleAsync(long? branch, DateTimeOffset start, DateTimeOffset end, CancellationToken ct)
    {
        var branchId = _user.SeesAllBranches ? branch : _user.BranchId;
        if (branchId is null) throw new ValidationException("branch", "Indique la sede.");
        var chapels = await _db.Chapels.AsNoTracking().Where(c => c.BranchId == branchId).OrderBy(c => c.Name).ToListAsync(ct);
        var ids = chapels.Select(c => c.Id).ToList();
        var bookings = await (from b in _db.ChapelBookings.AsNoTracking()
                              join s in _db.Services.AsNoTracking() on b.ServiceId equals s.Id
                              where ids.Contains(b.ChapelId) && b.StartsAt < end && start < b.EndsAt
                              select new { b.Id, b.ChapelId, s.Code, b.StartsAt, b.EndsAt }).ToListAsync(ct);
        return new ChapelCalendarDto(chapels.Select(c => new ChapelDayDto(c.Id, c.Name, c.Capacity,
            bookings.Where(b => b.ChapelId == c.Id).OrderBy(b => b.StartsAt).Select(b => new ChapelBookingDto(b.Id, b.Code, b.StartsAt, b.EndsAt)).ToList())).ToList());
    }
}
