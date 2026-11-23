/*
 * Archivo:    backend/src/Acompana.Application/Inventory/StockQuery.cs
 * Qué hace:   Stock de cofres por sede y modelo.
 * Recibe:     branch? + ICurrentUser.
 * Procesa:    BranchScope; marca nivel ok / min / out.
 * Devuelve:   StockRowDto[].
 * Se conecta: ← GET /api/stock · paso 4 del asistente (cofre sin stock deshabilitado)
 * Reglas:     RF-26 · RF-27
 * Estado:     Completo
 */

using Acompana.Application.Common;
using Microsoft.EntityFrameworkCore;

namespace Acompana.Application.Inventory;

public sealed class StockQuery
{
    private readonly IAppDbContext _db;
    private readonly ICurrentUser _user;
    public StockQuery(IAppDbContext db, ICurrentUser user) { _db = db; _user = user; }

    public async Task<IReadOnlyList<StockRowDto>> HandleAsync(long? branch, CancellationToken ct)
    {
        var rows = await (from s in _db.CasketStock.AsNoTracking().ForUser(_user, branch)
                          join b in _db.Branches.AsNoTracking() on s.BranchId equals b.Id
                          join m in _db.CasketModels.AsNoTracking() on s.CasketModelId equals m.Id
                          orderby b.Name, m.Sku
                          select new { s.BranchId, BranchName = b.Name, ModelId = m.Id, m.Sku, ModelName = m.Name, m.ImagePath, s.Qty, s.MinQty }).ToListAsync(ct);
        return rows.Select(r => new StockRowDto(r.BranchId, r.BranchName, r.ModelId, r.Sku, r.ModelName, r.ImagePath, r.Qty, r.MinQty,
            r.Qty <= 0 ? "out" : r.Qty <= r.MinQty ? "min" : "ok")).ToList();
    }
}
