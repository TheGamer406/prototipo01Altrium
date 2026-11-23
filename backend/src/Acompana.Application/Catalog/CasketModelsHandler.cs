/*
 * Archivo:    backend/src/Acompana.Application/Catalog/CasketModelsHandler.cs
 * Qué hace:   Listar, crear y editar modelos de cofre sin programar (gerencia).
 * Recibe:     filtros (active?, category?, branch?) para listar; DTO para crear/editar.
 * Procesa:    CRUD simple; list acepta branch para incluir stock de la sede; solo rol Management
 *               escribe.
 * Devuelve:   Lista de DTO o el DTO guardado.
 * Se conecta: ← CatalogController (GET/POST/PUT) · → AppDbContext
 * Reglas:     RF-04 · RF-26
 * Estado:     Parcial: solo lectura (la edición es de la fase 5)
 *
 * Contrato:
 *   Task<IReadOnlyList<TDto>> ListAsync(TFilter f, CancellationToken ct)
 *     Recibe:     filtros
 *     Procesa:    consulta
 *     Devuelve:   lista
 *   Task<TDto> SaveAsync(long? id, TDto dto, CancellationToken ct)
 *     Recibe:     id (null = nuevo) y datos
 *     Procesa:    valida y guarda
 *     Devuelve:   DTO guardado
 *     Errores:    403 si el rol no es Gerencia
 */

using Acompana.Application.Common;
using Microsoft.EntityFrameworkCore;

namespace Acompana.Application.Catalog;

public sealed class CasketModelsHandler
{
    private readonly IAppDbContext _db;
    private readonly ICurrentUser _user;
    public CasketModelsHandler(IAppDbContext db, ICurrentUser user) { _db = db; _user = user; }

    public async Task<IReadOnlyList<CasketModelDto>> ListAsync(long? branch, CancellationToken ct)
    {
        var branchId = _user.SeesAllBranches ? branch : _user.BranchId;
        var models = await _db.CasketModels.AsNoTracking().OrderBy(m => m.PriceCrc).ToListAsync(ct);
        var stock = branchId is null ? new Dictionary<long, int>()
            : await _db.CasketStock.AsNoTracking().Where(s => s.BranchId == branchId).ToDictionaryAsync(s => s.CasketModelId, s => s.Qty, ct);
        return models.Select(m => new CasketModelDto(m.Id, m.Sku, m.Name, m.Material, m.Origin.ToString(), m.PriceCrc, m.ImagePath,
            branchId is null ? null : stock.GetValueOrDefault(m.Id))).ToList();
    }
}
