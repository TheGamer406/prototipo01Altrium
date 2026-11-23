/*
 * Archivo:    backend/src/Acompana.Infrastructure/Seed/ServiceBuilder.cs
 * Qué hace:   Arma servicios completos (fallecido, familia, líneas, factura y pago) para el seed.
 * Recibe:     SeedContext, sede, paquete, fechas, estado y opciones (personalizado, aseguradora, destino).
 * Procesa:    1) crea fallecido y familiar con datos ficticios 2) copia las líneas del paquete con precio y
 *               exoneración del catálogo (cofre con modelo elegido) 3) opcionalmente personaliza
 *               4) AddInvoice / AddPayment crean la factura y el pago con IVA calculado por InvoiceCalculator.
 * Devuelve:   Service (sin guardar) listo para AddRange.
 * Se conecta: ← HistorySeed, TodaySeed · → InvoiceCalculator, entidades de Domain
 * Reglas:     RF-01 · RF-04 · RF-05 · RF-10 · RF-11
 * Estado:     Completo
 */
using Acompana.Application.Billing;
using Acompana.Domain.Billing;
using Acompana.Domain.Catalog;
using Acompana.Domain.Enums;
using Acompana.Domain.Organization;
using Acompana.Domain.Services;

namespace Acompana.Infrastructure.Seed;

public static class ServiceBuilder
{
    private static readonly string[] Relationships = { "Hijo", "Hija", "Esposo", "Esposa", "Hermano", "Hermana", "Nieto", "Sobrina" };
    private static readonly InvoiceCalculator Calc = new();

    public static string Advisor(Branch b) => $"Asesor · {b.Name}";

    public static Service Build(SeedContext c, Branch branch, Package pkg, DateTimeOffset scheduledAt, DateTimeOffset createdAt,
        ServiceKind kind, ServiceStatus status, bool custom = false, bool insurer = false, bool? cremation = null,
        string? familyNationalId = null)
    {
        var f = c.Faker;
        var born = new DateOnly(c.Rng.Next(1928, 1992), c.Rng.Next(1, 13), c.Rng.Next(1, 28));
        var deceased = new Deceased
        {
            FullName = $"{f.Name.FirstName()} {f.Name.LastName()} {f.Name.LastName()}",
            NationalId = c.NationalId(), BirthDate = born,
            DeathDate = kind == ServiceKind.Advance ? null : DateOnly.FromDateTime(scheduledAt.UtcDateTime.AddDays(-1)),
            CauseNote = kind == ServiceKind.Advance ? "Servicio anticipado" : c.Pick(new[] { "Causas naturales", "Enfermedad", "Accidente", "Insuficiencia" }),
            CreatedAt = createdAt,
        };
        var famName = $"{f.Name.FirstName()} {f.Name.LastName()} {deceased.FullName.Split(' ').Last()}";
        var family = new FamilyContact
        {
            FullName = famName, NationalId = familyNationalId ?? c.NationalId(), Relationship = c.Pick(Relationships),
            Phone = c.Phone(), Email = $"familia{c.Rng.Next(1000, 99999)}@correo.example.cr", CreatedAt = createdAt,
        };
        if (!insurer && c.Rng.Next(100) < 40) family.SetConsent(true, createdAt);

        var destination = (cremation ?? pkg.Name == "Cremación") ? FinalDestination.Cremation : FinalDestination.Cemetery;
        var wake = pkg.Items.Any(i => i.Component?.Category == ComponentCategory.Church) && c.Rng.Next(3) == 0 ? WakePlace.Church
            : c.Rng.Next(10) == 0 ? WakePlace.Home : WakePlace.Chapel;

        var s = new Service
        {
            Code = c.NextCode(createdAt.Year), BranchId = branch.Id, Kind = kind, Status = status,
            Deceased = deceased, FamilyContact = family, BasePackageId = pkg.Id,
            WakePlace = wake, WakeAddress = wake == WakePlace.Home ? $"{f.Address.StreetAddress()}, {branch.Canton}" : null,
            FinalDestination = destination,
            CemeteryName = destination == FinalDestination.Cemetery ? c.Pick(new[] { "Cementerio General", "Cementerio Metropolitano", "Cementerio Jardines del Recuerdo", "Cementerio Municipal" }) : null,
            ScheduledAt = scheduledAt, Payer = insurer ? Payer.Insurer : Payer.Family,
            InsurerId = insurer ? c.Pick(c.Insurers).Id : null,
            PolicyNumber = insurer ? $"POL-{c.Rng.Next(100000, 999999)}" : null,
            CreatedAt = createdAt,
        };
        if (wake != WakePlace.Chapel && wake != WakePlace.Home) s.WakeAddress = null;

        foreach (var it in pkg.Items)
            s.Lines.Add(NewLine(c, s, it.Component!, it.Qty));

        if (custom)
        {
            s.IsCustomPackage = true;
            var removable = s.Lines.Where(l => l.Component is { } k && k.Category is ComponentCategory.Flowers or ComponentCategory.Catering or ComponentCategory.Music).ToList();
            if (removable.Count > 0 && c.Rng.Next(2) == 0) s.Lines.Remove(c.Pick(removable));
            else
            {
                var extras = c.Components.Where(k => k.Category is ComponentCategory.Music or ComponentCategory.Flowers or ComponentCategory.Catering
                                                     && s.Lines.All(l => l.ComponentId != k.Id)).ToList();
                if (extras.Count > 0) s.Lines.Add(NewLine(c, s, c.Pick(extras), 1));
            }
        }

        foreach (var l in s.Lines.Where(l => l.Component?.RequiresProvider == true)) l.DueAt = scheduledAt.AddHours(-2);
        s.RecalculateTotal();
        return s;
    }

    public static ServiceLine NewLine(SeedContext c, Service s, Component comp, int qty)
    {
        var unit = comp.PriceCrc;
        long? model = null;
        if (comp.Category == ComponentCategory.Casket)
        {
            // el modelo elegido define el precio de la línea
            var pool = comp.PriceCrc <= 400_000 ? c.Caskets.Take(3) : comp.PriceCrc <= 700_000 ? c.Caskets.Skip(1).Take(4) : c.Caskets.Skip(4);
            var m = c.Pick(pool.ToList());
            model = m.Id; unit = m.PriceCrc;
        }
        return new ServiceLine
        {
            ServiceId = s.Id, ComponentId = comp.Id, Component = comp, CasketModelId = model, Qty = qty,
            UnitPriceCrc = unit, TaxExemptPct = comp.TaxExemptPct, LineTotalCrc = qty * unit, Status = LineStatus.Pending,
            CreatedAt = s.CreatedAt,
        };
    }

    public static Invoice AddInvoice(SeedContext c, Service s, InvoiceStatus status, DateTimeOffset sentAt, string? rejectReason = null, string? receiverIdOverride = null)
    {
        var t = Calc.Calculate(s.Lines.Select(l => (l.Qty, l.UnitPriceCrc, l.TaxExemptPct)));
        var insurer = s.Payer == Payer.Insurer ? c.Insurers.First(i => i.Id == s.InsurerId) : null;
        c.InvoiceSeq++;
        var inv = new Invoice
        {
            ServiceId = s.Id, Number = $"FAC-{sentAt.Year}-{c.InvoiceSeq:D6}",
            ReceiverName = insurer?.Name ?? s.FamilyContact!.FullName,
            ReceiverNationalId = receiverIdOverride ?? (insurer is null ? s.FamilyContact!.NationalId : $"3-101-{c.Rng.Next(100000, 999998)}"),
            SubtotalCrc = t.Subtotal, ExemptCrc = t.Exempt, TaxCrc = t.Tax, TotalCrc = t.Total,
            Status = status, IdempotencyKey = $"{s.Id}-1", SentAt = sentAt, CreatedAt = sentAt,
        };
        if (status == InvoiceStatus.Accepted)
        {
            c.CoreSeq++;
            inv.CoreReference = $"FE-{c.CoreSeq:D10}";
            inv.AcceptedAt = sentAt.AddSeconds(c.Rng.Next(20, 90));
        }
        if (status == InvoiceStatus.Rejected) inv.RejectReason = rejectReason ?? "Identificación del receptor inválida";
        s.Invoices.Add(inv);
        return inv;
    }

    public static Payment AddPayment(SeedContext c, Invoice inv, Service s, DateTimeOffset paidAt)
    {
        var p = new Payment
        {
            InvoiceId = inv.Id, AmountCrc = inv.TotalCrc, PaidAt = paidAt, CreatedAt = paidAt,
            Method = s.Payer == Payer.Insurer ? PaymentMethod.Insurer : c.Pick(new[] { PaymentMethod.Card, PaymentMethod.Transfer, PaymentMethod.Cash })
        };
        inv.Payments.Add(p);
        return p;
    }

    public static void History(Service s, ServiceStatus from, ServiceStatus to, DateTimeOffset at) =>
        s.History.Add(new StatusHistory { ServiceId = s.Id, FromStatus = from, ToStatus = to, ChangedBy = ServiceBuilderActor(s), ChangedAt = at, CreatedAt = at });

    private static string ServiceBuilderActor(Service s) => s.Branch is { } b ? Advisor(b) : "Asesor";
}
