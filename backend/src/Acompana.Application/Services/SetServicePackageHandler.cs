/*
 * Archivo:    backend/src/Acompana.Application/Services/SetServicePackageHandler.cs
 * Qué hace:   Cambia el paquete base y/o las líneas del servicio (personalización).
 * Recibe:     id + { basePackageId, lines[{componentId, casketModelId?, qty}] }.
 * Procesa:    1) solo si Status ∈ {Advance, Registered}
 *             2) reemplaza líneas copiando precio y exoneración del catálogo; el Package base no se
 *               toca
 *             3) IsCustomPackage = líneas ≠ ítems del paquete base (también si solo queda el cofre)
 *             4) RecalculateTotal
 * Devuelve:   ServiceDetailDto con líneas y total nuevos.
 * Se conecta: ← PUT /api/services/{id}/package · → AuditInterceptor (caso de las flores)
 * Reglas:     RF-04 · RF-05 · RF-10
 * Estado:     Completo
 *
 * Contrato:
 *   Task<ServiceDetailDto> HandleAsync(Guid id, SetPackageRequest req, CancellationToken ct)
 *     Recibe:     paquete base y líneas deseadas
 *     Procesa:    pasos 1-4
 *     Devuelve:   detalle
 *     Errores:    422 'El paquete ya no se puede cambiar: el servicio está facturado.'
 */

using Acompana.Application.Common;
using Acompana.Domain.Common;
using Acompana.Domain.Enums;
using Microsoft.EntityFrameworkCore;

namespace Acompana.Application.Services;

public sealed class SetServicePackageHandler
{
    private readonly IAppDbContext _db;
    private readonly ICurrentUser _user;
    private readonly IOpsNotifier _notifier;
    private readonly GetServiceDetailQuery _detail;

    public SetServicePackageHandler(IAppDbContext db, ICurrentUser user, IOpsNotifier notifier, GetServiceDetailQuery detail)
    { _db = db; _user = user; _notifier = notifier; _detail = detail; }

    public async Task<ServiceDetailDto> HandleAsync(Guid id, SetPackageRequest req, CancellationToken ct)
    {
        var s = await _db.Services.Include(x => x.Lines).ThenInclude(l => l.Component).FirstOrDefaultAsync(x => x.Id == id, ct);
        if (s is null || (!_user.SeesAllBranches && s.BranchId != _user.BranchId)) throw new NotFoundException("El servicio no existe.");
        if (s.Status is not (ServiceStatus.Advance or ServiceStatus.Registered))
            throw new DomainException("El paquete ya no se puede cambiar: el servicio está facturado.", "package-locked");

        var package = await _db.Packages.Include(p => p.Items).FirstOrDefaultAsync(p => p.Id == req.BasePackageId, ct)
                      ?? throw new ValidationException("basePackageId", "Seleccione un paquete.");
        var resolved = await ServiceLineFactory.ResolveAsync(_db, s.BranchId, req.Lines, ct);

        // Diferencias: solo se borra/agrega lo que cambió, así la bitácora muestra exactamente qué línea se quitó (RF-31)
        var existing = s.Lines.ToList();
        foreach (var (input, comp, model) in resolved)
        {
            var match = existing.FirstOrDefault(l => l.ComponentId == comp.Id);
            if (match is null) { s.Lines.Add(ServiceLineFactory.NewLine(s.Id, comp, model, input.Qty, s.ScheduledAt)); continue; }
            existing.Remove(match);
            var unit = model?.PriceCrc ?? (comp.Category == ComponentCategory.Casket && match.CasketModelId == null ? comp.PriceCrc : match.CasketModelId != null && model == null ? comp.PriceCrc : match.UnitPriceCrc);
            if (match.Qty != input.Qty || match.CasketModelId != model?.Id || match.UnitPriceCrc != unit)
            {
                match.Qty = input.Qty; match.CasketModelId = model?.Id; match.UnitPriceCrc = unit; match.LineTotalCrc = input.Qty * unit;
            }
        }
        foreach (var gone in existing) { s.Lines.Remove(gone); _db.ServiceLines.Remove(gone); }

        s.BasePackageId = package.Id;
        s.IsCustomPackage = ServiceLineFactory.IsCustom(package, s.Lines.Select(l => (l.ComponentId, l.Qty)));
        s.RecalculateTotal();
        await _db.SaveChangesAsync(ct);
        await _notifier.ServiceUpdated(s.Id, s.BranchId);
        return await _detail.HandleAsync(s.Id, ct);
    }
}
