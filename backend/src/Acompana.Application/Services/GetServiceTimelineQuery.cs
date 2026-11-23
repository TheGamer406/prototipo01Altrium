/*
 * Archivo:    backend/src/Acompana.Application/Services/GetServiceTimelineQuery.cs
 * Qué hace:   Línea de tiempo del servicio (estados, cambios y correos).
 * Recibe:     id.
 * Procesa:    Une StatusHistory + AuditLog del servicio + OutboundMessages y ordena por fecha.
 * Devuelve:   TimelineEntryDto[] ordenado ascendente.
 * Se conecta: ← GET /api/services/{id}/timeline
 * Reglas:     RF-19 · RF-31 · RF-35
 * Estado:     Completo
 */

using Acompana.Application.Common;
using Microsoft.EntityFrameworkCore;

namespace Acompana.Application.Services;

public sealed class GetServiceTimelineQuery
{
    private readonly IAppDbContext _db;
    private readonly ICurrentUser _user;
    public GetServiceTimelineQuery(IAppDbContext db, ICurrentUser user) { _db = db; _user = user; }

    public async Task<IReadOnlyList<TimelineEntryDto>> HandleAsync(Guid id, CancellationToken ct)
    {
        var s = await _db.Services.AsNoTracking().FirstOrDefaultAsync(x => x.Id == id, ct);
        if (s is null || (!_user.SeesAllBranches && s.BranchId != _user.BranchId)) throw new NotFoundException("El servicio no existe.");

        var status = await _db.StatusHistory.AsNoTracking().Where(h => h.ServiceId == id).ToListAsync(ct);
        var audit = await _db.AuditLog.AsNoTracking().Where(a => a.ServiceId == id).ToListAsync(ct);
        var mail = await _db.OutboundMessages.AsNoTracking().Where(m => m.ServiceId == id).ToListAsync(ct);

        var all = new List<TimelineEntryDto>();
        all.AddRange(status.Select(h => new TimelineEntryDto(h.ChangedAt, "status", $"{Label(h.FromStatus.ToString())} → {Label(h.ToStatus.ToString())} · {h.ChangedBy}")));
        all.AddRange(audit.Select(a => new TimelineEntryDto(a.At, "audit", $"{ActionLabel(a.Action.ToString())} {EntityLabel(a.Entity)} · {a.ActorName}")));
        all.AddRange(mail.Select(m => new TimelineEntryDto(m.CreatedAt, "message", $"Correo a {m.ToAddress}: {m.Subject}")));
        return all.OrderBy(e => e.At).ToList();
    }

    private static string Label(string s) => s switch
    {
        "Advance" => "Anticipado", "Registered" => "Registrado", "Invoicing" => "En facturación", "Paid" => "Pagado",
        "InProgress" => "En curso", "Completed" => "Finalizado", "Cancelled" => "Cancelado", _ => s,
    };
    private static string ActionLabel(string a) => a switch { "Create" => "Creó", "Delete" => "Eliminó", _ => "Modificó" };
    private static string EntityLabel(string e) => e switch { "ServiceLine" => "un componente", "Service" => "el servicio", "DeathCertificate" => "el acta", _ => e };
}
