/*
 * Archivo:    backend/src/Acompana.Api/Realtime/SignalROpsNotifier.cs
 * Qué hace:   Implementa IOpsNotifier enviando eventos al hub.
 * Recibe:     Llamadas de handlers y jobs con el payload del evento.
 * Procesa:    IHubContext<OpsHub>.Clients.Groups(sede, 'management').SendAsync(evento, payload).
 * Devuelve:   Task.
 * Se conecta: ← Application (IOpsNotifier) · → OpsHub
 * Reglas:     RNF-06
 * Estado:     Completo
 */

using Acompana.Api.Hubs;
using Acompana.Application.Common;
using Microsoft.AspNetCore.SignalR;

namespace Acompana.Api.Realtime;

public sealed class SignalROpsNotifier : IOpsNotifier
{
    private readonly IHubContext<OpsHub> _hub;
    public SignalROpsNotifier(IHubContext<OpsHub> hub) => _hub = hub;

    private IClientProxy Targets(long? branchId) =>
        branchId is null ? _hub.Clients.Group("management") : _hub.Clients.Groups("management", $"branch-{branchId}");

    public Task ServiceUpdated(Guid serviceId, long? branchId) => Targets(branchId).SendAsync("ServiceUpdated", new { serviceId });
    public Task OfferAwarded(Guid lineId, string providerName, decimal price, DateTimeOffset at, long? branchId) =>
        _hub.Clients.All.SendAsync("OfferAwarded", new { lineId, providerName, price, at });
    public Task AlertRaised(object alert, long? branchId) => Targets(branchId).SendAsync("AlertRaised", alert);
    public Task CremationMoved(long cremationId, string status) => _hub.Clients.All.SendAsync("CremationMoved", new { cremationId, status });
    public Task StockChanged(long branchId, long modelId, int qty) => Targets(branchId).SendAsync("StockChanged", new { branchId, modelId, qty });
}
