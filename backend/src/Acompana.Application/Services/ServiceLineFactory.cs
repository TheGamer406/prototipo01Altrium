/*
 * Archivo:    backend/src/Acompana.Application/Services/ServiceLineFactory.cs
 * Qué hace:   Construye líneas de servicio desde el catálogo y detecta si el paquete quedó personalizado.
 * Recibe:     IAppDbContext, sede, líneas pedidas (componente, modelo de cofre, cantidad) y fecha del servicio.
 * Procesa:    1) valida cantidades y que el componente exista y esté activo
 *             2) copia precio y % exento del catálogo (el cofre toma el precio del modelo elegido)
 *             3) exige existencias del modelo de cofre en la sede (RF-26)
 *             4) DueAt = fecha − 2 h para componentes con proveedor (RF-19)
 * Devuelve:   Lista de ServiceLine (sin guardar) · IsCustom(paquete, líneas).
 * Se conecta: ← CreateServiceHandler, SetServicePackageHandler
 * Reglas:     RF-04 · RF-05 · RF-10 · RF-26
 * Estado:     Completo
 */
using Acompana.Domain.Catalog;
using Acompana.Domain.Enums;
using Acompana.Domain.Services;
using Microsoft.EntityFrameworkCore;
using Acompana.Application.Common;

namespace Acompana.Application.Services;

public static class ServiceLineFactory
{
    public static async Task<List<(LineInput Input, Component Component, CasketModel? Model)>> ResolveAsync(
        IAppDbContext db, long branchId, IEnumerable<LineInput> inputs, CancellationToken ct)
    {
        var list = inputs.ToList();
        if (list.Count == 0) throw new ValidationException("lines", "Seleccione al menos un componente.");
        if (list.Any(l => l.Qty < 1)) throw new ValidationException("lines", "La cantidad mínima es 1.");

        var compIds = list.Select(l => l.ComponentId).Distinct().ToList();
        var comps = await db.Components.Where(c => compIds.Contains(c.Id)).ToDictionaryAsync(c => c.Id, ct);
        var modelIds = list.Where(l => l.CasketModelId != null).Select(l => l.CasketModelId!.Value).Distinct().ToList();
        var models = await db.CasketModels.Where(m => modelIds.Contains(m.Id)).ToDictionaryAsync(m => m.Id, ct);
        var stock = await db.CasketStock.Where(s => s.BranchId == branchId && modelIds.Contains(s.CasketModelId)).ToDictionaryAsync(s => s.CasketModelId, ct);

        var result = new List<(LineInput, Component, CasketModel?)>();
        foreach (var l in list)
        {
            if (!comps.TryGetValue(l.ComponentId, out var comp) || !comp.Active)
                throw new ValidationException("lines", "Uno de los componentes ya no está disponible.");
            CasketModel? model = null;
            if (l.CasketModelId is { } mid)
            {
                if (comp.Category != ComponentCategory.Casket) throw new ValidationException("lines", "Solo el componente de cofre admite un modelo.");
                if (!models.TryGetValue(mid, out model)) throw new ValidationException("lines", "El modelo de cofre no existe.");
                if (!stock.TryGetValue(mid, out var st) || st.Qty <= 0)
                    throw new ValidationException("lines", $"El modelo {model.Name} no tiene existencias en esta sede.");
            }
            result.Add((l, comp, model));
        }
        return result;
    }

    public static ServiceLine NewLine(Guid serviceId, Component comp, CasketModel? model, int qty, DateTimeOffset scheduledAt)
    {
        var unit = model?.PriceCrc ?? comp.PriceCrc;
        return new ServiceLine
        {
            ServiceId = serviceId, ComponentId = comp.Id, Component = comp, CasketModelId = model?.Id, CasketModel = model, Qty = qty,
            UnitPriceCrc = unit, TaxExemptPct = comp.TaxExemptPct, LineTotalCrc = qty * unit, Status = LineStatus.Pending,
            DueAt = comp.RequiresProvider ? scheduledAt.AddHours(-2) : null,
        };
    }

    /// <summary>RF-05: el paquete es personalizado si los componentes o cantidades difieren del paquete base.</summary>
    public static bool IsCustom(Package basePackage, IEnumerable<(long ComponentId, int Qty)> lines)
    {
        var a = basePackage.Items.GroupBy(i => i.ComponentId).ToDictionary(g => g.Key, g => g.Sum(i => i.Qty));
        var b = lines.GroupBy(l => l.ComponentId).ToDictionary(g => g.Key, g => g.Sum(l => l.Qty));
        return a.Count != b.Count || a.Any(kv => !b.TryGetValue(kv.Key, out var q) || q != kv.Value);
    }
}
