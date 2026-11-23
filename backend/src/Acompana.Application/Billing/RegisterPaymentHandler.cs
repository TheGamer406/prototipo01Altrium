/*
 * Archivo:    backend/src/Acompana.Application/Billing/RegisterPaymentHandler.cs
 * Qué hace:   Registra el pago de una factura aceptada y arranca la operación del servicio.
 * Recibe:     invoiceId + PayInvoiceRequest.
 * Procesa:    1) exige Invoice.Status = Accepted y monto = total
 *             2) crea Payment; Service.ChangeStatus(Paid)
 *             3) DeductCasketHandler (stock + inventario externo, RF-26/27/28)
 *             4) DispatchOffersHandler (ofertas a proveedores, RF-15)
 *             todo en una transacción; las llamadas externas después del commit
 * Devuelve:   InvoiceDto con pago; el servicio queda Paid.
 * Se conecta: ← POST /api/invoices/{id}/pay · → DeductCasketHandler, DispatchOffersHandler,
 *               IOpsNotifier
 * Reglas:     RF-08 · RF-15 · RF-26
 * Estado:     Completo
 *
 * Contrato:
 *   Task<InvoiceDto> HandleAsync(Guid invoiceId, PayInvoiceRequest req, CancellationToken ct)
 *     Recibe:     factura, método y monto
 *     Procesa:    pasos 1-4
 *     Devuelve:   factura pagada
 *     Errores:    422 'La factura no ha sido aceptada por el core contable.' · 422 'El monto no
 *                   coincide.'
 */

using Acompana.Application.Common;
using Acompana.Application.Inventory;
using Acompana.Application.Providers;
using Acompana.Application.Services;
using Acompana.Domain.Billing;
using Acompana.Domain.Common;
using Acompana.Domain.Enums;
using Microsoft.EntityFrameworkCore;

namespace Acompana.Application.Billing;

public sealed class RegisterPaymentHandler
{
    private readonly IAppDbContext _db;
    private readonly ICurrentUser _user;
    private readonly IClock _clock;
    private readonly DeductCasketHandler _deduct;
    private readonly DispatchOffersHandler _dispatch;
    private readonly IOpsNotifier _notifier;
    private readonly FamilyNotifier _family;

    public RegisterPaymentHandler(IAppDbContext db, ICurrentUser user, IClock clock, DeductCasketHandler deduct,
        DispatchOffersHandler dispatch, IOpsNotifier notifier, FamilyNotifier family)
    { _db = db; _user = user; _clock = clock; _deduct = deduct; _dispatch = dispatch; _notifier = notifier; _family = family; }

    public async Task<InvoiceDto> HandleAsync(Guid invoiceId, PayInvoiceRequest req, CancellationToken ct)
    {
        var inv = await _db.Invoices.Include(i => i.Payments).FirstOrDefaultAsync(i => i.Id == invoiceId, ct) ?? throw new NotFoundException("La factura no existe.");
        var s = await _db.Services.Include(x => x.Lines).ThenInclude(l => l.Component).Include(x => x.FamilyContact).FirstAsync(x => x.Id == inv.ServiceId, ct);
        if (!_user.SeesAllBranches && s.BranchId != _user.BranchId) throw new NotFoundException("La factura no existe.");
        if (inv.Status != InvoiceStatus.Accepted) throw new DomainException("La factura no ha sido aceptada por el core contable.", "invoice-not-accepted");
        if (inv.Payments.Count > 0) throw new DomainException("La factura ya fue pagada.", "already-paid");
        if (req.AmountCrc != inv.TotalCrc) throw new DomainException("El monto no coincide.", "amount-mismatch");

        var now = _clock.Now;
        DeductOutcome? outcome;
        await using (var tx = await _db.BeginTransactionAsync(ct))
        {
            inv.Payments.Add(new Payment { InvoiceId = inv.Id, Method = req.Method, AmountCrc = req.AmountCrc, PaidAt = now });
            s.ChangeStatus(ServiceStatus.Paid, _user.DisplayName, now);
            outcome = await _deduct.ApplyAsync(s.Id, s.BranchId, ct);
            await _db.SaveChangesAsync(ct);
            await _dispatch.HandleAsync(s.Id, ct);
            await tx.CommitAsync(ct);
        }

        await _notifier.ServiceUpdated(s.Id, s.BranchId);
        if (outcome is not null) await _deduct.ReportAsync(outcome, ct);
        await _family.InProgressAsync(s, ct);
        return InvoiceMapper.ToDto(inv, s.Lines);
    }
}
