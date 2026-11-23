/*
 * Archivo:    backend/src/Acompana.Application/Providers/OfferTimeoutChecker.cs
 * Qué hace:   Detecta líneas sin aceptar dentro del plazo configurable.
 * Recibe:     now + Setting ProviderAcceptTimeoutMin (30 por defecto).
 * Procesa:    Líneas Offered con primera oferta enviada hace más del plazo y sin alerta previa:
 *               ofertas → Expired, crea Alert ProviderTimeout para la sede.
 * Devuelve:   int alertas creadas.
 * Se conecta: ← Api/Workers/OpsMonitorWorker (cada 30 s) · → IOpsNotifier (AlertRaised)
 * Reglas:     RF-17
 * Estado:     Completo
 */

using Acompana.Application.Common;
using Acompana.Application.Oversight;
using Acompana.Domain.Enums;
using Microsoft.EntityFrameworkCore;

namespace Acompana.Application.Providers;

public sealed class OfferTimeoutChecker
{
    private readonly IAppDbContext _db;
    private readonly IClock _clock;
    private readonly SettingsHandler _settings;
    private readonly AlertService _alerts;
    public OfferTimeoutChecker(IAppDbContext db, IClock clock, SettingsHandler settings, AlertService alerts) { _db = db; _clock = clock; _settings = settings; _alerts = alerts; }

    public async Task<int> HandleAsync(CancellationToken ct)
    {
        var timeout = await _settings.GetIntAsync("ProviderAcceptTimeoutMin", 30, ct);
        var limit = _clock.Now.AddMinutes(-timeout);
        var lines = await (from l in _db.ServiceLines
                           join o in _db.ProviderOffers on l.Id equals o.ServiceLineId
                           where l.Status == LineStatus.Offered && l.ProviderId == null && o.Status == OfferStatus.Open && o.SentAt < limit
                           select l.Id).Distinct().ToListAsync(ct);
        var created = 0;
        foreach (var lineId in lines)
        {
            var line = await _db.ServiceLines.AsNoTracking().Include(l => l.Component).FirstAsync(l => l.Id == lineId, ct);
            var service = await _db.Services.AsNoTracking().FirstAsync(s => s.Id == line.ServiceId, ct);
            await _db.ExecuteSqlAsync($"UPDATE provider_offers SET status = 'Expired', responded_at = now(), updated_at = now() WHERE service_line_id = {lineId} AND status = 'Open'", ct);
            var already = await _db.Alerts.AnyAsync(a => a.ServiceId == service.Id && a.Type == AlertType.ProviderTimeout && a.Message.Contains(line.Component!.Name), ct);
            if (already) continue;
            await _alerts.RaiseAsync(AlertType.ProviderTimeout, service.BranchId, service.Id,
                $"Ningún proveedor aceptó {line.Component!.Name} de {service.Code} en {timeout} min. Asigne uno manualmente.", ct);
            created++;
        }
        return created;
    }
}
