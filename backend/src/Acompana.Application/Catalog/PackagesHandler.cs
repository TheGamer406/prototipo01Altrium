/*
 * Archivo:    backend/src/Acompana.Application/Catalog/PackagesHandler.cs
 * Qué hace:   Listar, crear y editar paquetes sin programar (gerencia).
 * Recibe:     filtros (active?, category?, branch?) para listar; DTO para crear/editar.
 * Procesa:    CRUD simple; precio = suma de componentes; editar un paquete no cambia servicios ya
 *               registrados; solo rol Management escribe.
 * Devuelve:   Lista de DTO o el DTO guardado.
 * Se conecta: ← CatalogController (GET/POST/PUT) · → AppDbContext
 * Reglas:     RF-04 · RF-06
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

public sealed class PackagesHandler
{
    private readonly IAppDbContext _db;
    public PackagesHandler(IAppDbContext db) => _db = db;

    public async Task<IReadOnlyList<PackageDto>> ListAsync(bool? active, CancellationToken ct)
    {
        var q = _db.Packages.AsNoTracking().Include(p => p.Items).ThenInclude(i => i.Component).AsQueryable();
        if (active is not null) q = q.Where(p => p.Active == active);
        var rows = await q.OrderBy(p => p.Id).ToListAsync(ct);
        return rows.Select(p => new PackageDto(p.Id, p.Name, p.Description, p.PriceCrc,
            p.Items.Select(i => new PackageItemDto(i.ComponentId, i.Component!.Name, i.Component.Category.ToString(), i.Qty, i.Component.PriceCrc, i.Component.TaxExemptPct, i.Component.RequiresProvider)).ToList(),
            p.ImagePath, p.Active)).ToList();
    }
}
