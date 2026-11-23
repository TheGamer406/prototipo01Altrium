/*
 * Archivo:    backend/src/Acompana.Application/Catalog/ComponentsHandler.cs
 * Qué hace:   Listar, crear y editar componentes sin programar (gerencia).
 * Recibe:     filtros (active?, category?, branch?) para listar; DTO para crear/editar.
 * Procesa:    CRUD simple; valida TaxExemptPct ∈ {0,50,100}; solo rol Management escribe.
 * Devuelve:   Lista de DTO o el DTO guardado.
 * Se conecta: ← CatalogController (GET/POST/PUT) · → AppDbContext
 * Reglas:     RF-06 · RF-10
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
using Acompana.Domain.Enums;
using Microsoft.EntityFrameworkCore;

namespace Acompana.Application.Catalog;

public sealed class ComponentsHandler
{
    private readonly IAppDbContext _db;
    public ComponentsHandler(IAppDbContext db) => _db = db;

    public async Task<IReadOnlyList<ComponentDto>> ListAsync(string? category, bool? active, CancellationToken ct)
    {
        var q = _db.Components.AsNoTracking().AsQueryable();
        if (Enum.TryParse<ComponentCategory>(category, true, out var c)) q = q.Where(x => x.Category == c);
        if (active is not null) q = q.Where(x => x.Active == active);
        var rows = await q.OrderBy(x => x.Category).ThenBy(x => x.Name).ToListAsync(ct);
        return rows.Select(x => new ComponentDto(x.Id, x.Name, x.Category.ToString(), x.PriceCrc, x.TaxExemptPct, x.RequiresProvider, x.ProviderType?.ToString(), x.ImagePath, x.Active)).ToList();
    }
}
