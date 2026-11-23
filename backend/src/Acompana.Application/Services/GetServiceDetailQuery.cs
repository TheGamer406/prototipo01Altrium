/*
 * Archivo:    backend/src/Acompana.Application/Services/GetServiceDetailQuery.cs
 * Qué hace:   Detalle completo de un servicio.
 * Recibe:     id + ICurrentUser.
 * Procesa:    Include de líneas, acta, factura, familia; valida que el usuario vea la sede.
 * Devuelve:   ServiceDetailDto o 404 (también 404 si es de otra sede: no se revela que existe).
 * Se conecta: ← GET /api/services/{id}
 * Reglas:     RF-01 · RF-30
 * Estado:     Completo
 */

using Acompana.Application.Billing;
using Acompana.Application.Common;
using Acompana.Domain.Services;
using Microsoft.EntityFrameworkCore;

namespace Acompana.Application.Services;

public sealed class GetServiceDetailQuery
{
    private readonly IAppDbContext _db;
    private readonly ICurrentUser _user;
    private readonly IClock _clock;

    public GetServiceDetailQuery(IAppDbContext db, ICurrentUser user, IClock clock) { _db = db; _user = user; _clock = clock; }

    public async Task<ServiceDetailDto> HandleAsync(Guid id, CancellationToken ct)
    {
        var s = await _db.Services.AsNoTracking().AsSplitQuery()
            .Include(x => x.Branch).Include(x => x.Deceased).Include(x => x.FamilyContact).Include(x => x.BasePackage).Include(x => x.Insurer)
            .Include(x => x.Lines).ThenInclude(l => l.Component)
            .Include(x => x.Lines).ThenInclude(l => l.CasketModel)
            .Include(x => x.DeathCertificate)
            .Include(x => x.Invoices).ThenInclude(i => i.Payments)
            .FirstOrDefaultAsync(x => x.Id == id, ct);
        if (s is null || (!_user.SeesAllBranches && s.BranchId != _user.BranchId))
            throw new NotFoundException("El servicio no existe.");

        var providerIds = s.Lines.Where(l => l.ProviderId != null).Select(l => l.ProviderId!.Value).Distinct().ToList();
        var providers = await _db.Providers.AsNoTracking().Where(p => providerIds.Contains(p.Id)).ToDictionaryAsync(p => p.Id, p => p.Name, ct);
        var booking = await _db.ChapelBookings.AsNoTracking().Include(b => b.Chapel).Where(b => b.ServiceId == id).FirstOrDefaultAsync(ct);
        return Map(s, providers, booking, _clock.Now);
    }

    public static ServiceLineDto MapLine(ServiceLine l, IReadOnlyDictionary<long, string> providers, DateTimeOffset now) =>
        new(l.Id, l.ComponentId, l.Component?.Name ?? "", l.Component?.Category.ToString() ?? "", l.CasketModel?.ImagePath ?? l.Component?.ImagePath,
            l.Qty, l.UnitPriceCrc, l.TaxExemptPct, l.LineTotalCrc, l.Status.ToString(),
            l.ProviderId is { } p && providers.TryGetValue(p, out var n) ? n : null,
            l.AcceptedPriceCrc, l.AcceptedAt, l.DueAt, l.IsUrgent(now), l.CasketModelId, l.CasketModel?.Name);

    public static ServiceDetailDto Map(Service s, IReadOnlyDictionary<long, string> providers, Domain.Logistics.ChapelBooking? booking, DateTimeOffset now)
    {
        var lines = s.Lines.OrderBy(l => l.Component?.Category).ThenBy(l => l.Component?.Name).Select(l => MapLine(l, providers, now)).ToList();
        var invoice = s.Invoices.OrderByDescending(i => i.CreatedAt).FirstOrDefault();
        var d = s.Deceased!; var f = s.FamilyContact!;
        return new ServiceDetailDto(
            s.Id, s.Code, s.BranchId, s.Branch?.Name ?? "", s.Status.ToString(), s.Kind.ToString(), s.ScheduledAt, s.TotalCrc, s.IsCustomPackage,
            lines.Any(l => l.IsUrgent),
            new DeceasedDto(d.FullName, d.NationalId, d.BirthDate, d.DeathDate, d.CauseNote),
            new FamilyDto(f.FullName, f.NationalId, f.Relationship, f.Phone, f.Email, f.InsuranceProspectConsent),
            s.BasePackageId, s.BasePackage?.Name, lines,
            s.DeathCertificate is { } c ? new DeathCertificateDto(c.Number, $"/api/services/{s.Id}/death-certificate/file", c.RegisteredBy, c.RegisteredAt) : null,
            invoice is null ? null : InvoiceMapper.ToDto(invoice, s.Lines),
            new WakeDto(s.WakePlace.ToString(), s.WakeAddress, booking?.ChapelId, booking?.Chapel?.Name, booking?.StartsAt, booking?.EndsAt),
            new DestinationDto(s.FinalDestination.ToString(), s.CemeteryName),
            new PayerDto(s.Payer.ToString(), s.InsurerId, s.Insurer?.Name, s.PolicyNumber), s.Version);
    }
}
