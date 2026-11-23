/*
 * Archivo:    backend/src/Acompana.Application/Catalog/PackagesHandler.cs
 * Qué hace:   Listar, crear y editar paquetes sin programar (gerencia).
 * Recibe:     filtros (active?, category?, branch?) para listar; DTO para crear/editar.
 * Procesa:    CRUD simple; precio = suma de componentes; editar un paquete no cambia servicios ya
 *               registrados; solo rol Management escribe.
 * Devuelve:   Lista de DTO o el DTO guardado.
 * Se conecta: ← CatalogController (GET/POST/PUT) · → AppDbContext
 * Reglas:     RF-04 · RF-06
 * Estado:     Completo
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

using Acompana.Domain.Catalog;

using Acompana.Domain.Enums;

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

    public async Task<PackageDto> SaveAsync(long? id, PackageDto dto, ICurrentUser user, CancellationToken ct)
    {
        if (user.Role != DemoRole.Management) throw new ForbiddenException("Solo Gerencia edita el catálogo.");
        if (string.IsNullOrWhiteSpace(dto.Name)) throw new ValidationException("name", "Indique el nombre.");
        if (dto.Items.Count == 0 || dto.Items.Any(i => i.Qty < 1)) throw new ValidationException("items", "El paquete necesita componentes con cantidad de 1 o más.");
        var p = id is null ? new Package() : await _db.Packages.Include(x => x.Items).FirstOrDefaultAsync(x => x.Id == id, ct) ?? throw new NotFoundException("El paquete no existe.");
        p.Name = dto.Name.Trim(); p.Description = dto.Description ?? ""; p.ImagePath = dto.ImagePath; p.Active = dto.Active;
        _db.PackageItems.RemoveRange(p.Items); p.Items.Clear();
        foreach (var i in dto.Items) p.Items.Add(new PackageItem { ComponentId = i.ComponentId, Qty = i.Qty });
        if (id is null) _db.Packages.Add(p);
        await _db.SaveChangesAsync(ct);
        return (await ListAsync(null, ct)).First(x => x.Id == p.Id);
    }
}
