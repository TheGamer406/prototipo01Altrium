/*
 * Archivo:    backend/src/Acompana.Application/Providers/ProvidersHandler.cs
 * Qué hace:   Cartera de proveedores: listar, crear, editar, activar/desactivar.
 * Recibe:     filtros (type?, zone?, active?) o ProviderDto.
 * Procesa:    CRUD con BranchScope por zona.
 * Devuelve:   Lista o DTO guardado.
 * Se conecta: ← GET/POST/PUT /api/providers
 * Reglas:     RF-14
 * Estado:     Completo
 */

using Acompana.Application.Common;
using Acompana.Domain.Enums;
using Acompana.Domain.Providers;
using Microsoft.EntityFrameworkCore;

namespace Acompana.Application.Providers;

public sealed class ProvidersHandler
{
    private readonly IAppDbContext _db;
    private readonly ICurrentUser _user;
    private readonly IClock _clock;
    public ProvidersHandler(IAppDbContext db, ICurrentUser user, IClock clock) { _db = db; _user = user; _clock = clock; }

    public async Task<IReadOnlyList<ProviderDto>> ListAsync(string? type, string? zone, bool? active, CancellationToken ct)
    {
        var q = _db.Providers.AsNoTracking().AsQueryable();
        if (Enum.TryParse<ProviderType>(type, true, out var t)) q = q.Where(p => p.Type == t);
        if (Enum.TryParse<Zone>(zone, true, out var z)) q = q.Where(p => p.Zone == z);
        if (active is not null) q = q.Where(p => p.Active == active);
        var since = _clock.Now.AddDays(-30);
        var won = await _db.ServiceLines.AsNoTracking().Where(l => l.ProviderId != null && l.AcceptedAt >= since)
            .GroupBy(l => l.ProviderId!.Value).Select(g => new { Id = g.Key, N = g.Count() }).ToDictionaryAsync(x => x.Id, x => x.N, ct);
        var rows = await q.OrderBy(p => p.Type).ThenBy(p => p.Name).ToListAsync(ct);
        return rows.Select(p => Map(p, won.GetValueOrDefault(p.Id))).ToList();
    }

    public async Task<ProviderDto> SaveAsync(long? id, ProviderDto dto, CancellationToken ct)
    {
        if (_user.Role is not (DemoRole.Management or DemoRole.BranchManager)) throw new ForbiddenException();
        if (string.IsNullOrWhiteSpace(dto.Name)) throw new ValidationException("name", "Indique el nombre.");
        if (!Enum.TryParse<ProviderType>(dto.Type, true, out var type)) throw new ValidationException("type", "Tipo no válido.");
        if (!Enum.TryParse<Zone>(dto.Zone, true, out var zone)) throw new ValidationException("zone", "Zona no válida.");
        var p = id is null ? new Provider() : await _db.Providers.FirstOrDefaultAsync(x => x.Id == id, ct) ?? throw new NotFoundException("El proveedor no existe.");
        p.Name = dto.Name.Trim(); p.Type = type; p.Zone = zone; p.Email = dto.Email.Trim(); p.Phone = dto.Phone.Trim(); p.Active = dto.Active;
        if (id is null) _db.Providers.Add(p);
        await _db.SaveChangesAsync(ct);
        return Map(p, 0);
    }

    private static ProviderDto Map(Provider p, int won) => new(p.Id, p.Name, p.Type.ToString(), p.Zone.ToString(), p.Email, p.Phone, p.Active, won);
}
