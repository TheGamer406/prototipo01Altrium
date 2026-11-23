/*
 * Archivo:    backend/src/Acompana.Application/Common/IOpsNotifier.cs
 * Qué hace:   Contrato para avisar a las pantallas en tiempo real.
 * Recibe:     Eventos: ServiceUpdated(serviceId), OfferAwarded(lineId, providerName, price, at),
 *               AlertRaised(AlertDto), CremationMoved(cremationId, status), StockChanged(branchId,
 *               modelId, qty).
 * Procesa:    —
 * Devuelve:   Task (fire and forget, no bloquea la transacción).
 * Se conecta: ← handlers y jobs · → Api/Realtime/SignalROpsNotifier (hub /hubs/ops)
 * Reglas:     RF-17 · RF-19 · RF-29 · RNF-06
 * Estado:     Completo
 */

namespace Acompana.Application.Common;

public interface IOpsNotifier
{
    Task ServiceUpdated(Guid serviceId, long? branchId);
    Task OfferAwarded(Guid lineId, string providerName, decimal price, DateTimeOffset at, long? branchId);
    Task AlertRaised(object alert, long? branchId);
    Task CremationMoved(long cremationId, string status);
    Task StockChanged(long branchId, long modelId, int qty);
}

/// <summary>Notificador vacío para pruebas y cuando no hay hub.</summary>
public sealed class NullOpsNotifier : IOpsNotifier
{
    public Task ServiceUpdated(Guid serviceId, long? branchId) => Task.CompletedTask;
    public Task OfferAwarded(Guid lineId, string providerName, decimal price, DateTimeOffset at, long? branchId) => Task.CompletedTask;
    public Task AlertRaised(object alert, long? branchId) => Task.CompletedTask;
    public Task CremationMoved(long cremationId, string status) => Task.CompletedTask;
    public Task StockChanged(long branchId, long modelId, int qty) => Task.CompletedTask;
}
