/*
 * Archivo:    backend/src/Acompana.Application/Billing/IssueInvoiceHandler.cs
 * Qué hace:   Arma la factura del servicio con IVA y exoneraciones y la envía al core contable.
 * Recibe:     serviceId + ICurrentUser.
 * Procesa:    1) carga servicio y líneas; exige acta si es inmediato (RF-03)
 *             2) receptor = familia, o aseguradora si Payer = Insurer (RF-11)
 *             3) InvoiceCalculator; crea/reutiliza Invoice Draft con IdempotencyKey = serviceId +
 *               intento
 *             4) Status Sent; ChangeStatus(Invoicing); notifica ServiceUpdated ('Enviando…')
 *             5) ICoreAccountingClient.SendInvoiceAsync → Accepted (CoreReference) o Rejected (motivo
 *               + alerta InvoiceRejected)
 *             6) si aseguradora: IInsurerClient.SendNoticeAsync (RF-12); correo a la familia con la
 *               factura
 * Devuelve:   InvoiceDto con el estado final.
 * Se conecta: ← BillingController POST /api/services/{id}/invoice · → ICoreAccountingClient,
 *               IInsurerClient, FamilyNotifier, AppDbContext (invoices, service_lines), IOpsNotifier
 * Reglas:     RF-03 · RF-09 · RF-10 · RF-11 · RF-12 · RF-13 (Idempotency-Key)
 * Estado:     Completo
 *
 * Contrato:
 *   Task<InvoiceDto> HandleAsync(Guid serviceId, CancellationToken ct)
 *     Recibe:     id del servicio
 *     Procesa:    pasos 1-6
 *     Devuelve:   factura
 *     Errores:    422 'Registre el acta de defunción antes de facturar.' · 409 'El servicio ya tiene
 *                   factura aceptada.' · 503 'El core contable no responde; se reintentará.'
 */

using Acompana.Application.Common;
using Acompana.Application.Integrations;
using Acompana.Application.Oversight;
using Acompana.Application.Services;
using Acompana.Domain.Billing;
using Acompana.Domain.Common;
using Acompana.Domain.Enums;
using Microsoft.EntityFrameworkCore;

namespace Acompana.Application.Billing;

public sealed class IssueInvoiceHandler
{
    private readonly IAppDbContext _db;
    private readonly ICurrentUser _user;
    private readonly IClock _clock;
    private readonly InvoiceCalculator _calc;
    private readonly ICoreAccountingClient _core;
    private readonly IInsurerClient _insurer;
    private readonly FamilyNotifier _family;
    private readonly AlertService _alerts;
    private readonly IOpsNotifier _notifier;

    public IssueInvoiceHandler(IAppDbContext db, ICurrentUser user, IClock clock, InvoiceCalculator calc, ICoreAccountingClient core,
        IInsurerClient insurer, FamilyNotifier family, AlertService alerts, IOpsNotifier notifier)
    { _db = db; _user = user; _clock = clock; _calc = calc; _core = core; _insurer = insurer; _family = family; _alerts = alerts; _notifier = notifier; }

    public async Task<InvoiceDto> HandleAsync(Guid serviceId, CancellationToken ct)
    {
        var s = await _db.Services.Include(x => x.Lines).ThenInclude(l => l.Component).Include(x => x.FamilyContact).Include(x => x.Deceased)
            .Include(x => x.Insurer).Include(x => x.DeathCertificate).Include(x => x.Invoices).ThenInclude(i => i.Payments)
            .FirstOrDefaultAsync(x => x.Id == serviceId, ct);
        if (s is null || (!_user.SeesAllBranches && s.BranchId != _user.BranchId)) throw new NotFoundException("El servicio no existe.");
        if (s.Invoices.Any(i => i.Status == InvoiceStatus.Accepted)) throw new ConflictException("El servicio ya tiene factura aceptada.");
        if (s.Kind == ServiceKind.Immediate && s.DeathCertificate is null)
            throw new DomainException("Registre el acta de defunción antes de facturar.", "death-certificate-required");

        var now = _clock.Now;
        var totals = _calc.Calculate(s.Lines.Select(l => (l.Qty, l.UnitPriceCrc, l.TaxExemptPct)));
        var last = s.Invoices.OrderByDescending(i => i.CreatedAt).FirstOrDefault();
        var invoice = last is { Status: InvoiceStatus.Draft or InvoiceStatus.Sent } ? last : null;
        if (invoice is null)
        {
            var seq = await _db.Invoices.CountAsync(ct) + 1;
            invoice = new Invoice { ServiceId = s.Id, Number = $"FAC-{now.Year}-{seq:D6}", IdempotencyKey = $"{s.Id}-{s.Invoices.Count + 1}" };
            s.Invoices.Add(invoice);
        }
        var insurer = s.Payer == Payer.Insurer ? s.Insurer : null;
        invoice.ReceiverName = insurer?.Name ?? s.FamilyContact!.FullName;
        invoice.ReceiverNationalId = insurer is null ? s.FamilyContact!.NationalId : $"3-101-{100000 + insurer.Id * 10 + 1}";
        invoice.SubtotalCrc = totals.Subtotal; invoice.ExemptCrc = totals.Exempt; invoice.TaxCrc = totals.Tax; invoice.TotalCrc = totals.Total;
        invoice.MarkSent(now);
        if (s.Status == ServiceStatus.Registered) s.ChangeStatus(ServiceStatus.Invoicing, _user.DisplayName, now);
        else if (s.Status != ServiceStatus.Invoicing) throw new DomainException("Transición no permitida.", "invalid-transition");
        await _db.SaveChangesAsync(ct);
        await _notifier.ServiceUpdated(s.Id, s.BranchId);

        var request = new CoreInvoiceRequest(new CoreReceiver(invoice.ReceiverName, invoice.ReceiverNationalId),
            s.Lines.Select(l => new CoreInvoiceLine(l.Component?.Name ?? "Componente", l.Qty, l.UnitPriceCrc, l.TaxExemptPct)).ToList(),
            new CoreInvoiceTotals(totals.Subtotal, totals.Exempt, totals.Tax, totals.Total));
        var result = await _core.SendInvoiceAsync(request, invoice.IdempotencyKey, ct);

        if (result.Accepted)
        {
            invoice.MarkAccepted(result.CoreReference ?? "", _clock.Now);
            await _db.SaveChangesAsync(ct);
            if (insurer is not null)
            {
                try { await _insurer.SendNoticeAsync(new InsurerNotice(insurer.Name, s.PolicyNumber ?? "", s.Deceased!.FullName, s.Code, totals.Total), ct); }
                catch (IntegrationUnavailableException) { /* el aviso se puede reenviar; la factura ya quedó aceptada */ }
            }
            await _family.InvoiceAsync(s, invoice, ct);
        }
        else
        {
            invoice.MarkRejected(result.RejectReason ?? "Rechazada por el core contable");
            await _db.SaveChangesAsync(ct);
            await _alerts.RaiseAsync(AlertType.InvoiceRejected, s.BranchId, s.Id, $"El core contable rechazó la factura de {s.Code}: {invoice.RejectReason}.", ct);
        }
        await _notifier.ServiceUpdated(s.Id, s.BranchId);
        return InvoiceMapper.ToDto(invoice, s.Lines);
    }
}
