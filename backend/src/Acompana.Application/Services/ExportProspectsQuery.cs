/*
 * Archivo:    backend/src/Acompana.Application/Services/ExportProspectsQuery.cs
 * Qué hace:   Exporta familiares como prospectos de seguro, solo con consentimiento.
 * Recibe:     from, to (fechas) + rol Management.
 * Procesa:    FamilyContacts con InsuranceProspectConsent = true y ConsentAt en el rango.
 * Devuelve:   CSV (text/csv; UTF-8 con BOM) con nombre, teléfono, correo, fecha de consentimiento.
 * Se conecta: ← GET /api/prospects/export
 * Reglas:     RF-36 · RNF-14
 * Estado:     Completo
 */

using System.Text;
using Acompana.Application.Common;
using Acompana.Domain.Enums;
using Microsoft.EntityFrameworkCore;

namespace Acompana.Application.Services;

public sealed class ExportProspectsQuery
{
    private readonly IAppDbContext _db;
    private readonly ICurrentUser _user;
    public ExportProspectsQuery(IAppDbContext db, ICurrentUser user) { _db = db; _user = user; }

    public async Task<byte[]> HandleAsync(DateOnly start, DateOnly end, CancellationToken ct)
    {
        if (_user.Role != DemoRole.Management) throw new ForbiddenException("Solo Gerencia puede exportar prospectos.");
        var from = new DateTimeOffset(start.ToDateTime(TimeOnly.MinValue), TimeSpan.FromHours(-6)).ToUniversalTime();
        var to = new DateTimeOffset(end.AddDays(1).ToDateTime(TimeOnly.MinValue), TimeSpan.FromHours(-6)).ToUniversalTime();
        var rows = await _db.FamilyContacts.AsNoTracking()
            .Where(f => f.InsuranceProspectConsent && f.ConsentAt != null && f.ConsentAt >= from && f.ConsentAt < to)
            .OrderBy(f => f.ConsentAt).ToListAsync(ct);
        var sb = new StringBuilder("Nombre,Teléfono,Correo,Fecha de consentimiento\r\n");
        foreach (var f in rows)
            sb.Append($"\"{f.FullName.Replace("\"", "\"\"")}\",{f.Phone},{f.Email},{f.ConsentAt!.Value.ToOffset(TimeSpan.FromHours(-6)):dd/MM/yyyy}\r\n");
        return new UTF8Encoding(true).GetPreamble().Concat(Encoding.UTF8.GetBytes(sb.ToString())).ToArray();
    }
}
