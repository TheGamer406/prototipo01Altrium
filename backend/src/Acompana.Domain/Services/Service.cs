/*
 * Archivo:    backend/src/Acompana.Domain/Services/Service.cs
 * Qué hace:   Agregado principal: un servicio funerario (tabla `services`) con sus líneas, acta e
 *               historial.
 * Recibe:     Campos: Code (SRV-2026-00001), BranchId, Kind, Status, DeceasedId, FamilyContactId,
 *               BasePackageId, IsCustomPackage, WakePlace, WakeAddress?, FinalDestination,
 *               CemeteryName?, ScheduledAt, Payer, InsurerId?, PolicyNumber?, TotalCrc, Version (xmin).
 * Procesa:    ChangeStatus(to, actor): valida la transición permitida
 *               (Advance→Registered→Invoicing→Paid→InProgress→Completed; cualquiera→Cancelled) y agrega
 *               un StatusHistory.
 *             RecalculateTotal(): suma LineTotalCrc de las líneas.
 *             MarkCustom(): IsCustomPackage = true cuando cambian las líneas respecto al paquete base.
 * Devuelve:   Nada; lanza DomainException si la transición no es válida.
 * Se conecta: ← handlers de Services, Billing, Providers, Logistics · → ServiceLine, DeathCertificate,
 *               StatusHistory
 * Reglas:     RF-01 · RF-02 · RF-05 · RF-07 · RF-08 · RF-11
 * Estado:     Completo
 *
 * Contrato:
 *   void ChangeStatus(ServiceStatus to, string actor, DateTimeOffset now)
 *     Recibe:     estado destino, nombre del actor (del usuario demo), hora actual
 *     Procesa:    busca (Status, to) en la tabla de transiciones; si es Invoicing exige
 *                   DeathCertificate cuando Kind = Immediate (RF-03); si es InProgress exige Status =
 *                   Paid (RF-08)
 *     Devuelve:   void; agrega StatusHistory(from, to, actor, now)
 *     Errores:    DomainException 'Registre el acta de defunción antes de facturar.' · 'El servicio no
 *                   puede iniciar sin factura pagada.' · 'Transición no permitida.'
 *   decimal RecalculateTotal()
 *     Recibe:     nada (usa Lines)
 *     Procesa:    TotalCrc = Σ line.LineTotalCrc
 *     Devuelve:   el nuevo TotalCrc
 */

using Acompana.Domain.Billing;
using Acompana.Domain.Catalog;
using Acompana.Domain.Common;
using Acompana.Domain.Enums;
using Acompana.Domain.Organization;

namespace Acompana.Domain.Services;

public class Service : GuidEntity, IHasBranch
{
    private static readonly (ServiceStatus From, ServiceStatus To)[] Allowed =
    {
        (ServiceStatus.Advance, ServiceStatus.Registered),
        (ServiceStatus.Registered, ServiceStatus.Invoicing),
        (ServiceStatus.Invoicing, ServiceStatus.Paid),
        (ServiceStatus.Paid, ServiceStatus.InProgress),
        (ServiceStatus.InProgress, ServiceStatus.Completed),
    };

    public string Code { get; set; } = "";
    public long BranchId { get; set; }
    public Branch? Branch { get; set; }
    public ServiceKind Kind { get; set; }
    public ServiceStatus Status { get; set; }
    public long DeceasedId { get; set; }
    public Deceased? Deceased { get; set; }
    public long FamilyContactId { get; set; }
    public FamilyContact? FamilyContact { get; set; }
    public long? BasePackageId { get; set; }
    public Package? BasePackage { get; set; }
    public bool IsCustomPackage { get; set; }
    public WakePlace WakePlace { get; set; }
    public string? WakeAddress { get; set; }
    public FinalDestination FinalDestination { get; set; }
    public string? CemeteryName { get; set; }
    public DateTimeOffset ScheduledAt { get; set; }
    public Payer Payer { get; set; }
    public long? InsurerId { get; set; }
    public Insurer? Insurer { get; set; }
    public string? PolicyNumber { get; set; }
    public decimal TotalCrc { get; set; }
    public uint Version { get; set; }

    public List<ServiceLine> Lines { get; set; } = new();
    public DeathCertificate? DeathCertificate { get; set; }
    public List<StatusHistory> History { get; set; } = new();
    public List<Invoice> Invoices { get; set; } = new();

    public void ChangeStatus(ServiceStatus to, string actor, DateTimeOffset now)
    {
        if (to == ServiceStatus.Cancelled)
        {
            if (Status is ServiceStatus.Completed or ServiceStatus.Cancelled)
                throw new DomainException("Transición no permitida.", "invalid-transition");
        }
        else
        {
            if (!Allowed.Contains((Status, to)))
                throw new DomainException("Transición no permitida.", "invalid-transition");
            if (to == ServiceStatus.Invoicing && Kind == ServiceKind.Immediate && DeathCertificate is null)
                throw new DomainException("Registre el acta de defunción antes de facturar.", "death-certificate-required");
            if (to == ServiceStatus.InProgress && Status != ServiceStatus.Paid)
                throw new DomainException("El servicio no puede iniciar sin factura pagada.", "payment-required");
        }

        History.Add(new StatusHistory { ServiceId = Id, FromStatus = Status, ToStatus = to, ChangedBy = actor, ChangedAt = now });
        Status = to;
    }

    public decimal RecalculateTotal()
    {
        TotalCrc = Lines.Sum(l => l.LineTotalCrc);
        return TotalCrc;
    }

    public void MarkCustom() => IsCustomPackage = true;
}
