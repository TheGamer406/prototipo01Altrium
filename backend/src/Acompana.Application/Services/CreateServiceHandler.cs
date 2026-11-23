/*
 * Archivo:    backend/src/Acompana.Application/Services/CreateServiceHandler.cs
 * Qué hace:   Registra un servicio nuevo (inmediato o anticipado) desde el asistente.
 * Recibe:     CreateServiceRequest + ICurrentUser.
 * Procesa:    1) valida campos (FluentValidation-like a mano, mensajes en español)
 *             2) crea Deceased y FamilyContact (ConsentAt si hay consentimiento)
 *             3) crea Service con Code consecutivo SRV-{año}-{00000}, Status = Advance o Registered
 *             4) copia líneas: precio y % exento desde el catálogo; IsCustomPackage si difieren del
 *               paquete base
 *             5) si WakePlace = Chapel reserva la capilla (BookChapelHandler)
 *             6) RecalculateTotal y guarda en una transacción; notifica ServiceUpdated
 * Devuelve:   ServiceDetailDto del servicio creado (201 Created + Location).
 * Se conecta: ← ServicesController POST /api/services · → AppDbContext, BookChapelHandler,
 *               IOpsNotifier, FamilyNotifier (correo de registro)
 * Reglas:     RF-01 · RF-02 · RF-04 · RF-05 · RF-07 · RF-36
 * Estado:     Completo
 *
 * Contrato:
 *   Task<ServiceDetailDto> HandleAsync(CreateServiceRequest req, CancellationToken ct)
 *     Recibe:     la solicitud completa del asistente (6 pasos)
 *     Procesa:    pasos 1-6
 *     Devuelve:   detalle del servicio
 *     Errores:    400 validación por campo · 409 capilla ocupada (mensaje con la reserva que choca)
 */

using System.Text.RegularExpressions;
using Acompana.Application.Common;
using Acompana.Application.Logistics;
using Acompana.Domain.Enums;
using Acompana.Domain.Organization;
using Acompana.Domain.Services;
using Microsoft.EntityFrameworkCore;

namespace Acompana.Application.Services;

public sealed class CreateServiceHandler
{
    private static readonly Regex NationalId = new(@"^\d-\d{4}-\d{4}$");
    private static readonly Regex Email = new(@"^[^@\s]+@[^@\s]+\.[^@\s]+$");
    private static readonly Regex Phone = new(@"^\d{4}-?\d{4}$");

    private readonly IAppDbContext _db;
    private readonly ICurrentUser _user;
    private readonly IClock _clock;
    private readonly BookChapelHandler _chapel;
    private readonly IOpsNotifier _notifier;
    private readonly FamilyNotifier _family;
    private readonly GetServiceDetailQuery _detail;

    public CreateServiceHandler(IAppDbContext db, ICurrentUser user, IClock clock, BookChapelHandler chapel, IOpsNotifier notifier,
        FamilyNotifier family, GetServiceDetailQuery detail)
    { _db = db; _user = user; _clock = clock; _chapel = chapel; _notifier = notifier; _family = family; _detail = detail; }

    public async Task<ServiceDetailDto> HandleAsync(CreateServiceRequest req, CancellationToken ct)
    {
        var now = _clock.Now;
        var branchId = _user.SeesAllBranches ? req.BranchId : _user.BranchId ?? req.BranchId;
        Validate(req);

        var branch = await _db.Branches.AsNoTracking().FirstOrDefaultAsync(b => b.Id == branchId, ct)
                     ?? throw new ValidationException("branchId", "La sede no existe.");
        var package = await _db.Packages.Include(p => p.Items).FirstOrDefaultAsync(p => p.Id == req.BasePackageId && p.Active, ct)
                      ?? throw new ValidationException("basePackageId", "Seleccione un paquete.");
        var resolved = await ServiceLineFactory.ResolveAsync(_db, branchId, req.Lines, ct);

        var status = req.Kind == ServiceKind.Advance ? ServiceStatus.Advance : ServiceStatus.Registered;
        var deceased = new Deceased
        {
            FullName = req.Deceased.FullName.Trim(), NationalId = req.Deceased.NationalId, BirthDate = req.Deceased.BirthDate,
            DeathDate = req.Kind == ServiceKind.Advance ? null : req.Deceased.DeathDate,
            CauseNote = req.Kind == ServiceKind.Advance ? "Servicio anticipado" : "",
        };
        var family = new FamilyContact
        {
            FullName = req.Family.FullName.Trim(), NationalId = req.Family.NationalId, Relationship = req.Family.Relationship,
            Phone = req.Family.Phone, Email = req.Family.Email.Trim(),
        };
        family.SetConsent(req.Family.InsuranceProspectConsent, now);

        var service = new Service
        {
            BranchId = branchId, Kind = req.Kind, Status = status, Deceased = deceased, FamilyContact = family,
            BasePackageId = package.Id, WakePlace = req.WakePlace, WakeAddress = req.WakePlace == WakePlace.Chapel ? null : req.WakeAddress?.Trim(),
            FinalDestination = req.FinalDestination, CemeteryName = req.FinalDestination == FinalDestination.Cemetery ? req.CemeteryName?.Trim() : null,
            ScheduledAt = req.ScheduledAt.ToUniversalTime(), Payer = req.Payer,
            InsurerId = req.Payer == Payer.Insurer ? req.InsurerId : null, PolicyNumber = req.Payer == Payer.Insurer ? req.PolicyNumber?.Trim() : null,
        };
        foreach (var (input, comp, model) in resolved)
            service.Lines.Add(ServiceLineFactory.NewLine(service.Id, comp, model, input.Qty, service.ScheduledAt));
        service.IsCustomPackage = ServiceLineFactory.IsCustom(package, service.Lines.Select(l => (l.ComponentId, l.Qty)));
        service.RecalculateTotal();

        await using var tx = await _db.BeginTransactionAsync(ct);
        var year = _clock.Today.Year;
        service.Code = await NextCodeAsync(year, ct);
        _db.Services.Add(service);
        await _db.SaveChangesAsync(ct);

        if (req.WakePlace == WakePlace.Chapel)
        {
            if (req.ChapelId is null) throw new ValidationException("chapelId", "Seleccione una capilla.");
            var from = service.ScheduledAt;
            await _chapel.HandleAsync(new BookChapelRequest(req.ChapelId.Value, service.Id, from, from.AddHours(24)), ct);
        }
        await tx.CommitAsync(ct);

        await _notifier.ServiceUpdated(service.Id, service.BranchId);
        await _family.RegisteredAsync(service, branch.Name, ct);
        return await _detail.HandleAsync(service.Id, ct);
    }

    private async Task<string> NextCodeAsync(int year, CancellationToken ct)
    {
        var prefix = $"SRV-{year}-";
        var last = await _db.Services.Where(s => s.Code.StartsWith(prefix)).OrderByDescending(s => s.Code).Select(s => s.Code).FirstOrDefaultAsync(ct);
        var n = last is null ? 0 : int.Parse(last[prefix.Length..]);
        return $"{prefix}{n + 1:D5}";
    }

    private void Validate(CreateServiceRequest r)
    {
        var e = new Dictionary<string, string>();
        if (string.IsNullOrWhiteSpace(r.Deceased?.FullName)) e["deceased.fullName"] = "Indique el nombre del fallecido.";
        if (r.Deceased is null || !NationalId.IsMatch(r.Deceased.NationalId ?? "")) e["deceased.nationalId"] = "Use el formato 1-0234-0567.";
        if (r.Deceased is not null && r.Deceased.BirthDate > DateOnly.FromDateTime(_clock.Now.UtcDateTime)) e["deceased.birthDate"] = "La fecha de nacimiento no puede ser futura.";
        if (r.Kind == ServiceKind.Immediate)
        {
            if (r.Deceased?.DeathDate is null) e["deceased.deathDate"] = "Indique la fecha de defunción.";
            else if (r.Deceased.DeathDate > _clock.Today) e["deceased.deathDate"] = "La fecha de defunción no puede ser futura.";
            else if (r.Deceased.DeathDate < r.Deceased.BirthDate) e["deceased.deathDate"] = "La defunción no puede ser anterior al nacimiento.";
        }
        if (string.IsNullOrWhiteSpace(r.Family?.FullName)) e["family.fullName"] = "Indique el nombre del responsable.";
        if (r.Family is null || !NationalId.IsMatch(r.Family.NationalId ?? "")) e["family.nationalId"] = "Use el formato 1-0234-0567.";
        if (string.IsNullOrWhiteSpace(r.Family?.Relationship)) e["family.relationship"] = "Indique el parentesco.";
        if (r.Family is null || !Phone.IsMatch(r.Family.Phone ?? "")) e["family.phone"] = "Use el formato 8888-8888.";
        if (r.Family is null || !Email.IsMatch(r.Family.Email ?? "")) e["family.email"] = "Ingrese un correo válido.";
        if (r.ScheduledAt <= _clock.Now.AddMinutes(-5)) e["scheduledAt"] = "La fecha y hora debe ser futura.";
        if (r.WakePlace != WakePlace.Chapel && r.WakePlace == WakePlace.Home && string.IsNullOrWhiteSpace(r.WakeAddress)) e["wakeAddress"] = "Indique la dirección de la velación.";
        if (r.WakePlace == WakePlace.Chapel && r.ChapelId is null) e["chapelId"] = "Seleccione una capilla.";
        if (r.FinalDestination == FinalDestination.Cemetery && string.IsNullOrWhiteSpace(r.CemeteryName)) e["cemeteryName"] = "Indique el cementerio.";
        if (r.Payer == Payer.Insurer && r.InsurerId is null) e["insurerId"] = "Seleccione la aseguradora.";
        if (r.Payer == Payer.Insurer && string.IsNullOrWhiteSpace(r.PolicyNumber)) e["policyNumber"] = "Indique el número de póliza.";
        if (r.Lines is null || r.Lines.Count == 0) e["lines"] = "Seleccione al menos un componente.";
        if (e.Count > 0) throw new ValidationException(e);
    }
}
