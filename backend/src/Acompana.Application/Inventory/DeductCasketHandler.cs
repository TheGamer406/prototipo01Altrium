/*
 * Archivo:    backend/src/Acompana.Application/Inventory/DeductCasketHandler.cs
 * Qué hace:   Descuenta el cofre vendido y dispara alerta + reabastecimiento si llega al mínimo.
 * Recibe:     serviceId (lo llama RegisterPaymentHandler).
 * Procesa:    busca la línea Casket; CasketStock.Deduct(1) de la sede
 *             IInventoryClient.ReportMovementAsync(-1)
 *             si Qty <= MinQty: Alert LowStock (sede y bodega) + RestockRequest automática +
 *               IInventoryClient.RequestRestockAsync
 *             notifica StockChanged
 * Devuelve:   StockRowDto de la sede/modelo.
 * Se conecta: ← RegisterPaymentHandler · → IInventoryClient, IOpsNotifier
 * Reglas:     RF-26 · RF-27 · RF-28
 * Estado:     Completo
 */

using Acompana.Application.Common;
using Acompana.Application.Integrations;
using Acompana.Application.Oversight;
using Acompana.Domain.Enums;
using Acompana.Domain.Inventory;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Acompana.Application.Inventory;

/// <summary>Resultado local del descuento: lo que falta reportar a los sistemas externos tras el commit.</summary>
public record DeductOutcome(long BranchId, long ModelId, string Sku, string ModelName, int NewQty, int MinQty, RestockRequest? Restock);

public sealed class DeductCasketHandler
{
    private readonly IAppDbContext _db;
    private readonly IInventoryClient _inventory;
    private readonly IOpsNotifier _notifier;
    private readonly AlertService _alerts;
    private readonly ILogger<DeductCasketHandler> _log;

    public DeductCasketHandler(IAppDbContext db, IInventoryClient inventory, IOpsNotifier notifier, AlertService alerts, ILogger<DeductCasketHandler> log)
    { _db = db; _inventory = inventory; _notifier = notifier; _alerts = alerts; _log = log; }

    /// <summary>Parte local (dentro de la transacción del pago): descuenta y deja la solicitud de bodega preparada.</summary>
    public async Task<DeductOutcome?> ApplyAsync(Guid serviceId, long branchId, CancellationToken ct)
    {
        var line = await _db.ServiceLines.Include(l => l.CasketModel).FirstOrDefaultAsync(l => l.ServiceId == serviceId && l.CasketModelId != null, ct);
        if (line?.CasketModel is null) return null;
        var stock = await _db.CasketStock.FirstOrDefaultAsync(s => s.BranchId == branchId && s.CasketModelId == line.CasketModelId, ct);
        if (stock is null) return null;
        stock.Deduct(line.Qty);
        RestockRequest? restock = null;
        if (stock.IsAtOrBelowMin)
        {
            restock = new RestockRequest { BranchId = branchId, CasketModelId = stock.CasketModelId, Qty = Math.Max(4, stock.MinQty * 2), Status = RestockStatus.Requested, RequestedAt = DateTimeOffset.UtcNow };
            _db.RestockRequests.Add(restock);
        }
        return new DeductOutcome(branchId, stock.CasketModelId, line.CasketModel.Sku, line.CasketModel.Name, stock.Qty, stock.MinQty, restock);
    }

    /// <summary>Parte posterior al commit: informa al inventario externo, completa la solicitud y avisa (RF-26, RF-27, RF-28).</summary>
    public async Task ReportAsync(DeductOutcome o, CancellationToken ct)
    {
        try
        {
            await _inventory.ReportMovementAsync(o.BranchId, o.Sku, -1, ct);
            if (o.Restock is not null)
            {
                var number = await _inventory.RequestRestockAsync(o.BranchId, o.Sku, o.Restock.Qty, ct);
                var tracked = await _db.RestockRequests.FirstAsync(r => r.Id == o.Restock.Id, ct);
                tracked.ExternalNumber = number;
                await _db.SaveChangesAsync(ct);
            }
        }
        catch (IntegrationUnavailableException ex) { _log.LogWarning(ex, "Inventario externo no disponible; el saldo local es la fuente"); }

        if (o.Restock is not null)
        {
            var branch = await _db.Branches.AsNoTracking().Where(b => b.Id == o.BranchId).Select(b => b.Name).FirstAsync(ct);
            await _alerts.RaiseAsync(AlertType.LowStock, o.BranchId, null,
                $"El modelo {o.ModelName} ({o.Sku}) quedó en {o.NewQty} unidad(es) en {branch}; el mínimo es {o.MinQty}. Se solicitó reabastecimiento a la bodega central.", ct);
        }
        await _notifier.StockChanged(o.BranchId, o.ModelId, o.NewQty);
    }
}
