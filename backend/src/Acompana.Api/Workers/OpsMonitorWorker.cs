/*
 * Archivo:    backend/src/Acompana.Api/Workers/OpsMonitorWorker.cs
 * Qué hace:   Trabajo en segundo plano cada 30 s.
 * Recibe:     Nada; se ejecuta solo (BackgroundService con PeriodicTimer).
 * Procesa:    Crea un scope y llama OfferTimeoutChecker y TransferDelayChecker.
 * Devuelve:   Nada; las alertas salen por IOpsNotifier.
 * Se conecta: → Application/Providers/OfferTimeoutChecker, Application/Logistics/TransferDelayChecker
 * Reglas:     RF-17 · RF-24
 * Estado:     Parcial: falta TransferDelayChecker (fase 6)
 */

using Acompana.Application.Admin;
using Acompana.Application.Providers;

namespace Acompana.Api.Workers;

public sealed class OpsMonitorWorker : BackgroundService
{
    private readonly IServiceScopeFactory _scopes;
    private readonly ILogger<OpsMonitorWorker> _log;
    public OpsMonitorWorker(IServiceScopeFactory scopes, ILogger<OpsMonitorWorker> log) { _scopes = scopes; _log = log; }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        await Task.Delay(TimeSpan.FromSeconds(20), stoppingToken);
        using var timer = new PeriodicTimer(TimeSpan.FromSeconds(30));
        do
        {
            try
            {
                using var scope = _scopes.CreateScope();
                await scope.ServiceProvider.GetRequiredService<OfferTimeoutChecker>().HandleAsync(stoppingToken);
                await scope.ServiceProvider.GetRequiredService<RefreshAnalyticsHandler>().RefreshSnapshotAsync(stoppingToken);
            }
            catch (Exception ex) when (ex is not OperationCanceledException) { _log.LogWarning(ex, "Ciclo del monitor con error"); }
        } while (await timer.WaitForNextTickAsync(stoppingToken));
    }
}
