/*
 * Archivo:    backend/src/Acompana.Application/Oversight/AlertService.cs
 * Qué hace:   Crea alertas, las guarda y las envía en vivo a la sede y a Gerencia.
 * Recibe:     Tipo, sede, servicio opcional y mensaje en español.
 * Procesa:    Inserta Alert, guarda y llama IOpsNotifier.AlertRaised con el AlertDto.
 * Devuelve:   El AlertDto creado.
 * Se conecta: ← IssueInvoiceHandler, DeductCasketHandler, OfferTimeoutChecker, AssignStaffHandler
 * Reglas:     RF-17 · RF-24 · RF-25 · RF-27
 * Estado:     Completo
 */
using Acompana.Application.Common;
using Acompana.Domain.Enums;
using Acompana.Domain.Oversight;
using Microsoft.EntityFrameworkCore;

namespace Acompana.Application.Oversight;

public sealed class AlertService
{
    private readonly IAppDbContext _db;
    private readonly IOpsNotifier _notifier;
    public AlertService(IAppDbContext db, IOpsNotifier notifier) { _db = db; _notifier = notifier; }

    public async Task<AlertDto> RaiseAsync(AlertType type, long branchId, Guid? serviceId, string message, CancellationToken ct)
    {
        var alert = new Alert { Type = type, BranchId = branchId, ServiceId = serviceId, Message = message };
        _db.Alerts.Add(alert);
        await _db.SaveChangesAsync(ct);
        var branch = await _db.Branches.AsNoTracking().Where(b => b.Id == branchId).Select(b => b.Name).FirstOrDefaultAsync(ct) ?? "";
        var code = serviceId is null ? null : await _db.Services.AsNoTracking().Where(s => s.Id == serviceId).Select(s => s.Code).FirstOrDefaultAsync(ct);
        var dto = new AlertDto(alert.Id, type.ToString(), branch, code, message, alert.CreatedAt, null);
        await _notifier.AlertRaised(dto, branchId);
        return dto;
    }
}
