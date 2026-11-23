/*
 * Archivo:    backend/src/Acompana.Application/Billing/GetInvoiceQuery.cs
 * Qué hace:   Devuelve una factura con sus líneas.
 * Recibe:     invoiceId.
 * Procesa:    Consulta con BranchScope.
 * Devuelve:   InvoiceDto o 404.
 * Se conecta: ← GET /api/invoices/{id}
 * Reglas:     RF-09
 * Estado:     Completo
 */

using Acompana.Application.Common;
using Microsoft.EntityFrameworkCore;

namespace Acompana.Application.Billing;

public sealed class GetInvoiceQuery
{
    private readonly IAppDbContext _db;
    private readonly ICurrentUser _user;
    public GetInvoiceQuery(IAppDbContext db, ICurrentUser user) { _db = db; _user = user; }

    public async Task<InvoiceDto> HandleAsync(Guid invoiceId, CancellationToken ct)
    {
        var inv = await _db.Invoices.AsNoTracking().Include(i => i.Payments).FirstOrDefaultAsync(i => i.Id == invoiceId, ct)
                  ?? throw new NotFoundException("La factura no existe.");
        var s = await _db.Services.AsNoTracking().Include(x => x.Lines).ThenInclude(l => l.Component).FirstAsync(x => x.Id == inv.ServiceId, ct);
        if (!_user.SeesAllBranches && s.BranchId != _user.BranchId) throw new NotFoundException("La factura no existe.");
        return InvoiceMapper.ToDto(inv, s.Lines);
    }
}
