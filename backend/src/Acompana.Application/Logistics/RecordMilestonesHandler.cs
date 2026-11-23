/*
 * Archivo:    backend/src/Acompana.Application/Logistics/RecordMilestonesHandler.cs
 * Qué hace:   Recibe un lote de hitos del teléfono del chofer (incluye los marcados sin señal).
 * Recibe:     transferId + MilestoneBatch.
 * Procesa:    INSERT … ON CONFLICT (client_uuid) DO NOTHING por hito; SyncedAt = now; Source según el
 *               teléfono.
 * Devuelve:   { accepted: n, duplicates: m }.
 * Se conecta: ← POST /api/transfers/{id}/milestones · → TransferDelayChecker, IOpsNotifier
 * Reglas:     RF-22 · RNF-05
 * Estado:     Completo
 */

using Acompana.Application.Common;
using Acompana.Domain.Enums;
using Microsoft.EntityFrameworkCore;

namespace Acompana.Application.Logistics;

public sealed class RecordMilestonesHandler
{
    private readonly IAppDbContext _db;
    private readonly ICurrentUser _user;
    private readonly IClock _clock;
    private readonly IOpsNotifier _notifier;
    public RecordMilestonesHandler(IAppDbContext db, ICurrentUser user, IClock clock, IOpsNotifier notifier) { _db = db; _user = user; _clock = clock; _notifier = notifier; }

    public async Task<MilestoneResult> HandleAsync(long transferId, MilestoneBatch batch, CancellationToken ct)
    {
        var t = await _db.Transfers.AsNoTracking().FirstOrDefaultAsync(x => x.Id == transferId, ct) ?? throw new NotFoundException("El traslado no existe.");
        var s = await _db.Services.AsNoTracking().FirstAsync(x => x.Id == t.ServiceId, ct);
        if (!_user.SeesAllBranches && s.BranchId != _user.BranchId) throw new NotFoundException("El traslado no existe.");
        if (batch.Items is not { Count: > 0 }) throw new ValidationException("items", "No hay hitos para registrar.");

        var now = _clock.Now; int accepted = 0, dup = 0;
        foreach (var m in batch.Items.OrderBy(i => i.RecordedAt))
        {
            if (!Enum.TryParse<MilestoneType>(m.Type, true, out var type)) throw new ValidationException("type", "Tipo de hito no válido.");
            var source = Enum.TryParse<MilestoneSource>(m.Source, true, out var src) ? src : MilestoneSource.Online;
            var typeText = type.ToString(); var srcText = source.ToString(); var rec = m.RecordedAt.ToUniversalTime();
            // un mismo hito reenviado (mismo client_uuid) no se duplica (RNF-05)
            var rows = await _db.ExecuteSqlAsync($@"INSERT INTO transfer_milestones (transfer_id, type, recorded_at, synced_at, lat, lng, source, client_uuid, created_at, updated_at)
                VALUES ({transferId}, {typeText}, {rec}, {now}, {m.Lat}, {m.Lng}, {srcText}, {m.ClientUuid}, {now}, {now}) ON CONFLICT (client_uuid) DO NOTHING", ct);
            if (rows == 1) accepted++; else dup++;
        }

        if (accepted > 0)
        {
            var last = batch.Items.OrderBy(i => i.RecordedAt).Last();
            var done = batch.Items.Any(i => i.Type is "ArrivalWake" or "ArrivalDestination");
            var status = done ? "Done" : "OnRoute";
            await _db.ExecuteSqlAsync($"UPDATE transfers SET status = {status}, updated_at = now() WHERE id = {transferId} AND status <> 'Done'", ct);
            var hearse = done ? "Available" : "InUse";
            await _db.ExecuteSqlAsync($"UPDATE hearses SET status = {hearse}, updated_at = now() WHERE id = {t.HearseId}", ct);
            await _notifier.ServiceUpdated(s.Id, s.BranchId);
        }
        return new MilestoneResult(accepted, dup);
    }
}
