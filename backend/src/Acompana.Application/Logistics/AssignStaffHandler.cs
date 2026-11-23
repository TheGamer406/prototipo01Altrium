/*
 * Archivo:    backend/src/Acompana.Application/Logistics/AssignStaffHandler.cs
 * Qué hace:   Asigna personal del turno a un servicio y avisa traslapes (no bloquea).
 * Recibe:     AssignStaffRequest.
 * Procesa:    Exige servicio Paid o InProgress (RF-08); inserta; busca otra asignación del mismo
 *               empleado que se traslape; si hay, Alert StaffOverlap.
 * Devuelve:   AssignStaffResult { id, overlapWith? }.
 * Se conecta: ← POST /api/staff-assignments
 * Reglas:     RF-08 · RF-25
 * Estado:     Completo
 */

using Acompana.Application.Common;
using Acompana.Application.Oversight;
using Acompana.Domain.Common;
using Acompana.Domain.Enums;
using Acompana.Domain.Logistics;
using Microsoft.EntityFrameworkCore;

namespace Acompana.Application.Logistics;

public sealed class AssignStaffHandler
{
    private readonly IAppDbContext _db;
    private readonly ICurrentUser _user;
    private readonly AlertService _alerts;
    public AssignStaffHandler(IAppDbContext db, ICurrentUser user, AlertService alerts) { _db = db; _user = user; _alerts = alerts; }

    public async Task<StaffBoardDto> ListAsync(long? branch, CancellationToken ct)
    {
        var b = _user.SeesAllBranches ? branch : _user.BranchId;
        var emps = await _db.Employees.AsNoTracking().Where(e => b == null || e.BranchId == b).OrderBy(e => e.FullName).ToListAsync(ct);
        var ids = emps.Select(e => e.Id).ToList();
        var since = DateTimeOffset.UtcNow.AddDays(-2);
        var asg = await (from a in _db.StaffAssignments.AsNoTracking()
                         join s in _db.Services.AsNoTracking() on a.ServiceId equals s.Id
                         join e in _db.Employees.AsNoTracking() on a.EmployeeId equals e.Id
                         where ids.Contains(a.EmployeeId) && a.EndsAt > since
                         orderby a.StartsAt
                         select new StaffAssignmentDto(a.Id, s.Code, e.Id, e.FullName, a.StartsAt, a.EndsAt)).ToListAsync(ct);
        return new StaffBoardDto(emps.Select(e => new EmployeeDto(e.Id, e.FullName, e.Role.ToString(), e.Shift.ToString(), e.BranchId)).ToList(), asg);
    }

    public async Task<AssignStaffResult> HandleAsync(AssignStaffRequest req, CancellationToken ct)
    {
        if (req.To <= req.From) throw new ValidationException("to", "La hora final debe ser posterior a la inicial.");
        var s = await _db.Services.AsNoTracking().FirstOrDefaultAsync(x => x.Id == req.ServiceId, ct) ?? throw new NotFoundException("El servicio no existe.");
        if (!_user.SeesAllBranches && s.BranchId != _user.BranchId) throw new NotFoundException("El servicio no existe.");
        if (s.Status is not (ServiceStatus.Paid or ServiceStatus.InProgress))
            throw new DomainException("El servicio no puede iniciar sin factura pagada.", "payment-required");
        var emp = await _db.Employees.AsNoTracking().FirstOrDefaultAsync(e => e.Id == req.EmployeeId, ct) ?? throw new NotFoundException("El empleado no existe.");

        var start = req.From.ToUniversalTime(); var end = req.To.ToUniversalTime();
        var other = await (from a in _db.StaffAssignments.AsNoTracking()
                           join o in _db.Services.AsNoTracking() on a.ServiceId equals o.Id
                           where a.EmployeeId == req.EmployeeId && a.StartsAt < end && start < a.EndsAt && a.ServiceId != req.ServiceId
                           select o.Code).FirstOrDefaultAsync(ct);
        var row = new StaffAssignment { ServiceId = req.ServiceId, EmployeeId = req.EmployeeId, StartsAt = start, EndsAt = end };
        _db.StaffAssignments.Add(row);
        await _db.SaveChangesAsync(ct);
        if (other is not null)
            await _alerts.RaiseAsync(AlertType.StaffOverlap, s.BranchId, s.Id, $"{emp.FullName} queda asignado a {s.Code} y también a {other} en el mismo horario.", ct);
        return new AssignStaffResult(row.Id, other);
    }
}
