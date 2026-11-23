/*
 * Archivo:    backend/src/Acompana.Application/Logistics/TransferDelayChecker.cs
 * Qué hace:   Alerta traslados con tramos más largos que el umbral.
 * Recibe:     now + Setting TransferThresholdMin (20).
 * Procesa:    Para cada traslado activo, tiempo entre hitos consecutivos (o desde el último hasta
 *               now); si supera el umbral y no hay alerta, crea Alert TransferDelay.
 * Devuelve:   int alertas creadas.
 * Se conecta: ← OpsMonitorWorker (cada 30 s)
 * Reglas:     RF-24
 * Estado:     Completo
 */

using Acompana.Application.Common;
using Acompana.Application.Oversight;
using Acompana.Domain.Enums;
using Microsoft.EntityFrameworkCore;

namespace Acompana.Application.Logistics;

public sealed class TransferDelayChecker
{
    private readonly IAppDbContext _db;
    private readonly IClock _clock;
    private readonly SettingsHandler _settings;
    private readonly AlertService _alerts;
    public TransferDelayChecker(IAppDbContext db, IClock clock, SettingsHandler settings, AlertService alerts) { _db = db; _clock = clock; _settings = settings; _alerts = alerts; }

    public async Task<int> HandleAsync(CancellationToken ct)
    {
        var threshold = await _settings.GetIntAsync("TransferThresholdMin", 20, ct);
        var now = _clock.Now;
        var active = await _db.Transfers.AsNoTracking().Include(t => t.Milestones).Where(t => t.Status == TransferStatus.OnRoute).ToListAsync(ct);
        var created = 0;
        foreach (var t in active)
        {
            if (!TransfersQuery.Delayed(t, now, threshold)) continue;
            var s = await _db.Services.AsNoTracking().FirstAsync(x => x.Id == t.ServiceId, ct);
            if (await _db.Alerts.AnyAsync(a => a.ServiceId == s.Id && a.Type == AlertType.TransferDelay, ct)) continue;
            await _alerts.RaiseAsync(AlertType.TransferDelay, s.BranchId, s.Id, $"Traslado de {s.Code} con demora: más de {threshold} min sin registrar el siguiente hito.", ct);
            created++;
        }
        return created;
    }
}
