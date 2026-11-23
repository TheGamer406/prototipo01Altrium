/*
 * Archivo:    backend/src/Acompana.Application/Logistics/TransfersQuery.cs
 * Qué hace:   Traslados de la sede con última posición para el mapa.
 * Recibe:     branch?.
 * Procesa:    Traslados no terminados + hitos; delayed si algún tramo supera el umbral.
 * Devuelve:   TransferDto[].
 * Se conecta: ← GET /api/transfers
 * Reglas:     RF-23 · RF-24
 * Estado:     Completo
 */

using Acompana.Application.Common;
using Acompana.Application.Oversight;
using Acompana.Domain.Enums;
using Acompana.Domain.Logistics;
using Microsoft.EntityFrameworkCore;

namespace Acompana.Application.Logistics;

public sealed class TransfersQuery
{
    private readonly IAppDbContext _db;
    private readonly ICurrentUser _user;
    private readonly IClock _clock;
    private readonly SettingsHandler _settings;
    public TransfersQuery(IAppDbContext db, ICurrentUser user, IClock clock, SettingsHandler settings) { _db = db; _user = user; _clock = clock; _settings = settings; }

    public async Task<IReadOnlyList<TransferDto>> HandleAsync(long? branch, bool includeDone, CancellationToken ct)
    {
        var threshold = await _settings.GetIntAsync("TransferThresholdMin", 20, ct);
        var cutoff = _clock.Now.AddHours(-24);
        var rows = await (from t in _db.Transfers.AsNoTracking().Include(x => x.Milestones)
                          join s in _db.Services.AsNoTracking().ForUser(_user, branch) on t.ServiceId equals s.Id
                          join h in _db.Hearses.AsNoTracking() on t.HearseId equals h.Id
                          join d in _db.Employees.AsNoTracking() on t.DriverId equals d.Id
                          where t.Status != TransferStatus.Done || (includeDone && t.UpdatedAt >= cutoff) || includeDone
                          orderby t.Id descending
                          select new { t, s.Code, h.Plate, Driver = d.FullName }).Take(100).ToListAsync(ct);
        var now = _clock.Now;
        return rows.Select(r => Map(r.t, r.Code, r.Plate, r.Driver, Delayed(r.t, now, threshold))).ToList();
    }

    public static bool Delayed(Transfer t, DateTimeOffset now, int thresholdMin)
    {
        var ms = t.Milestones.OrderBy(m => m.RecordedAt).ToList();
        for (var i = 1; i < ms.Count; i++) if ((ms[i].RecordedAt - ms[i - 1].RecordedAt).TotalMinutes > thresholdMin) return true;
        return t.Status == TransferStatus.OnRoute && ms.Count > 0 && (now - ms[^1].RecordedAt).TotalMinutes > thresholdMin;
    }

    public static TransferDto Map(Transfer t, string code, string plate, string driver, bool delayed)
    {
        var pos = t.LastPosition();
        return new TransferDto(t.Id, code, plate, driver, t.Status.ToString(), pos?.Lat, pos?.Lng,
            t.Milestones.OrderBy(m => m.RecordedAt).Select(m => new MilestoneDto(m.ClientUuid, m.Type.ToString(), m.RecordedAt, m.SyncedAt, m.Lat, m.Lng, m.Source.ToString())).ToList(), delayed);
    }
}
