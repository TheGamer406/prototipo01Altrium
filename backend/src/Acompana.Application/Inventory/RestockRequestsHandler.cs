/*
 * Archivo:    backend/src/Acompana.Application/Inventory/RestockRequestsHandler.cs
 * Qué hace:   Lista y despacha solicitudes de reabastecimiento.
 * Recibe:     filtros (status?, branch?) · id para despachar.
 * Procesa:    Dispatch: Requested → Dispatched; al recibir suma al stock.
 * Devuelve:   RestockRequestDto[] · RestockRequestDto.
 * Se conecta: ← GET /api/restock-requests, POST /api/restock-requests/{id}/dispatch
 * Reglas:     RF-28
 * Estado:     Completo
 */

using Acompana.Application.Common;
using Acompana.Domain.Enums;
using Microsoft.EntityFrameworkCore;

namespace Acompana.Application.Inventory;

public sealed class RestockRequestsHandler
{
    private readonly IAppDbContext _db;
    private readonly ICurrentUser _user;
    private readonly IOpsNotifier _notifier;
    public RestockRequestsHandler(IAppDbContext db, ICurrentUser user, IOpsNotifier notifier) { _db = db; _user = user; _notifier = notifier; }

    public async Task<IReadOnlyList<RestockRequestDto>> ListAsync(string? status, long? branch, CancellationToken ct)
    {
        var q = _db.RestockRequests.AsNoTracking().ForUser(_user, branch);
        if (Enum.TryParse<RestockStatus>(status, true, out var st)) q = q.Where(r => r.Status == st);
        var rows = await (from r in q
                          join b in _db.Branches.AsNoTracking() on r.BranchId equals b.Id
                          join m in _db.CasketModels.AsNoTracking() on r.CasketModelId equals m.Id
                          orderby r.RequestedAt descending
                          select new { r.Id, Branch = b.Name, m.Sku, r.Qty, r.Status, r.RequestedAt, r.ExternalNumber }).ToListAsync(ct);
        return rows.Select(r => new RestockRequestDto(r.Id, r.Branch, r.Sku, r.Qty, r.Status.ToString(), r.RequestedAt, r.ExternalNumber)).ToList();
    }

    /// <summary>Requested → Dispatched; Dispatched → Received suma al stock.</summary>
    public async Task<RestockRequestDto> AdvanceAsync(long id, CancellationToken ct)
    {
        var r = await _db.RestockRequests.FirstOrDefaultAsync(x => x.Id == id, ct) ?? throw new NotFoundException("La solicitud no existe.");
        if (!_user.SeesAllBranches && r.BranchId != _user.BranchId) throw new NotFoundException("La solicitud no existe.");
        if (r.Status == RestockStatus.Requested) r.Dispatch();
        else
        {
            r.Receive();
            var stock = await _db.CasketStock.FirstAsync(s => s.BranchId == r.BranchId && s.CasketModelId == r.CasketModelId, ct);
            stock.Qty += r.Qty;
            await _notifier.StockChanged(r.BranchId, r.CasketModelId, stock.Qty);
        }
        await _db.SaveChangesAsync(ct);
        return (await ListAsync(null, r.BranchId, ct)).First(x => x.Id == id);
    }
}
