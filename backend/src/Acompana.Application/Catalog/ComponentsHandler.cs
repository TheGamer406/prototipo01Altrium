/*
 * Archivo:    backend/src/Acompana.Application/Catalog/ComponentsHandler.cs
 * Qué hace:   Listar, crear y editar componentes sin programar (gerencia).
 * Recibe:     filtros (active?, category?, branch?) para listar; DTO para crear/editar.
 * Procesa:    CRUD simple; valida TaxExemptPct ∈ {0,50,100}; solo rol Management escribe.
 * Devuelve:   Lista de DTO o el DTO guardado.
 * Se conecta: ← CatalogController (GET/POST/PUT) · → AppDbContext
 * Reglas:     RF-06 · RF-10
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
using Acompana.Domain.Enums;
using Microsoft.EntityFrameworkCore;

using Acompana.Domain.Catalog;

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

    public async Task<ComponentDto> SaveAsync(long? id, ComponentDto dto, ICurrentUser user, CancellationToken ct)
    {
        if (user.Role != DemoRole.Management) throw new ForbiddenException("Solo Gerencia edita el catálogo.");
        if (string.IsNullOrWhiteSpace(dto.Name)) throw new ValidationException("name", "Indique el nombre.");
        if (!Enum.TryParse<ComponentCategory>(dto.Category, true, out var cat)) throw new ValidationException("category", "Categoría no válida.");
        ProviderType? pt = Enum.TryParse<ProviderType>(dto.ProviderType, true, out var p) ? p : null;
        var c = id is null ? new Component() : await _db.Components.FirstOrDefaultAsync(x => x.Id == id, ct) ?? throw new NotFoundException("El componente no existe.");
        c.Name = dto.Name.Trim(); c.Category = cat; c.PriceCrc = dto.PriceCrc; c.TaxExemptPct = dto.TaxExemptPct;
        c.RequiresProvider = dto.RequiresProvider; c.ProviderType = dto.RequiresProvider ? pt : null; c.ImagePath = dto.ImagePath; c.Active = dto.Active;
        try { c.Validate(); } catch (Domain.Common.DomainException e) { throw new ValidationException(e.Code == "invalid-exemption" ? "taxExemptPct" : e.Code == "invalid-price" ? "priceCrc" : "providerType", e.Message); }
        if (id is null) _db.Components.Add(c);
        await _db.SaveChangesAsync(ct);
        return new ComponentDto(c.Id, c.Name, c.Category.ToString(), c.PriceCrc, c.TaxExemptPct, c.RequiresProvider, c.ProviderType?.ToString(), c.ImagePath, c.Active);
    }
}
