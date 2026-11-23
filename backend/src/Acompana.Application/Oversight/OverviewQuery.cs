/*
 * Archivo:    backend/src/Acompana.Application/Oversight/OverviewQuery.cs
 * Qué hace:   Vista central: todas las sedes con servicios en curso, capillas ocupadas e inventario.
 * Recibe:     ICurrentUser (Management ve todo; otro rol solo su sede).
 * Procesa:    Lee analytics.mv_branch_snapshot y la completa con alertas abiertas en vivo.
 * Devuelve:   OverviewDto.
 * Se conecta: ← GET /api/overview · se refresca con eventos del hub
 * Reglas:     RF-29 · RNF-06
 * Estado:     Completo
 */

using Acompana.Application.Common;
using Microsoft.EntityFrameworkCore;

namespace Acompana.Application.Oversight;

public sealed class OverviewQuery
{
    private readonly IAppDbContext _db;
    private readonly ICurrentUser _user;
    public OverviewQuery(IAppDbContext db, ICurrentUser user) { _db = db; _user = user; }

    private class Row
    {
        public long BranchId { get; set; }
        public string Code { get; set; } = "";
        public string Name { get; set; } = "";
        public string Zone { get; set; } = "";
        public double Lat { get; set; }
        public double Lng { get; set; }
        public int Ongoing { get; set; }
        public int ChapelsBusy { get; set; }
        public int ChapelsTotal { get; set; }
        public int LowStock { get; set; }
    }

    public async Task<OverviewDto> HandleAsync(CancellationToken ct)
    {
        var rows = await _db.SqlQueryRaw<Row>(@"SELECT branch_id AS ""branch_id"", code AS ""code"", name AS ""name"", zone AS ""zone"", lat AS ""lat"", lng AS ""lng"",
            ongoing AS ""ongoing"", chapels_busy AS ""chapels_busy"", chapels_total AS ""chapels_total"", low_stock AS ""low_stock"" FROM analytics.mv_branch_snapshot ORDER BY name").ToListAsync(ct);
        if (!_user.SeesAllBranches) rows = rows.Where(r => r.BranchId == _user.BranchId).ToList();
        var open = await _db.Alerts.AsNoTracking().Where(a => a.AcknowledgedAt == null).ForUser(_user, null).CountAsync(ct);
        var branches = rows.Select(r => new BranchOverviewDto(r.BranchId, r.Code, r.Name, r.Zone, r.Lat, r.Lng, r.Ongoing, r.ChapelsBusy, r.ChapelsTotal, r.LowStock)).ToList();
        return new OverviewDto(new OverviewKpisDto(branches.Sum(b => b.Ongoing), branches.Sum(b => b.ChapelsBusy), branches.Sum(b => b.LowStock), open), branches);
    }
}
