/*
 * Archivo:    backend/src/Acompana.Api/Middleware/DemoUserMiddleware.cs
 * Qué hace:   Convierte X-Demo-Role y X-Demo-Branch en el usuario actual del request.
 * Recibe:     Encabezados HTTP (o query ?role=&branch= para SignalR).
 * Procesa:    Parsea rol (default Advisor) y sede (default San José); Management sin sede = todas;
 *               guarda en DemoCurrentUser (scoped).
 * Devuelve:   Continúa el pipeline; 400 si el rol no existe.
 * Se conecta: → DemoCurrentUser (ICurrentUser) · ← Program.cs (UseMiddleware)
 * Reglas:     RF-30
 * Estado:     Completo
 */

using Acompana.Application.Common;
using Acompana.Domain.Enums;
using Microsoft.EntityFrameworkCore;

namespace Acompana.Api.Middleware;

public sealed class DemoUserMiddleware
{
    private static readonly Dictionary<DemoRole, string> Labels = new()
    {
        [DemoRole.Advisor] = "Asesor", [DemoRole.BranchManager] = "Encargado", [DemoRole.CrematoriumOperator] = "Operador",
        [DemoRole.Provider] = "Proveedor", [DemoRole.Management] = "Gerencia", [DemoRole.Driver] = "Chofer",
    };
    private static Dictionary<long, string> _branches = new();
    private static Dictionary<long, string> _providers = new();
    private static readonly SemaphoreSlim Gate = new(1, 1);

    private readonly RequestDelegate _next;
    public DemoUserMiddleware(RequestDelegate next) => _next = next;

    public async Task InvokeAsync(HttpContext http, DemoCurrentUser user, IAppDbContext db)
    {
        string? Get(string header, string query) =>
            http.Request.Headers.TryGetValue(header, out var h) && !string.IsNullOrWhiteSpace(h) ? h.ToString()
            : http.Request.Query.TryGetValue(query, out var q) && !string.IsNullOrWhiteSpace(q) ? q.ToString() : null;

        var roleText = Get("X-Demo-Role", "role");
        var role = DemoRole.Advisor;
        if (roleText is not null && !Enum.TryParse(roleText, true, out role))
        {
            http.Response.StatusCode = 400;
            await http.Response.WriteAsJsonAsync(new { title = "Rol no válido", detail = $"El rol '{roleText}' no existe.", status = 400 });
            return;
        }

        await EnsureCacheAsync(db, http.RequestAborted);
        long? branch = long.TryParse(Get("X-Demo-Branch", "branch"), out var b) ? b : null;
        if (role != DemoRole.Management && branch is null)
            branch = _branches.FirstOrDefault(x => x.Value == "San José Centro").Key is var sj && sj != 0 ? sj : null;
        long? provider = long.TryParse(Get("X-Demo-Provider", "provider"), out var p) ? p : null;

        user.Role = role;
        user.BranchId = role == DemoRole.Management ? branch : branch;
        user.ProviderId = role == DemoRole.Provider ? provider : null;
        user.DisplayName = role switch
        {
            DemoRole.Management => "Gerencia",
            DemoRole.Provider => provider is not null && _providers.TryGetValue(provider.Value, out var pn) ? $"Proveedor · {pn}" : "Proveedor",
            _ => branch is not null && _branches.TryGetValue(branch.Value, out var bn) ? $"{Labels[role]} · {bn}" : Labels[role],
        };
        await _next(http);
    }

    private static async Task EnsureCacheAsync(IAppDbContext db, CancellationToken ct)
    {
        if (_branches.Count > 0) return;
        await Gate.WaitAsync(ct);
        try
        {
            if (_branches.Count > 0) return;
            _branches = await db.Branches.AsNoTracking().ToDictionaryAsync(x => x.Id, x => x.Name, ct);
            _providers = await db.Providers.AsNoTracking().ToDictionaryAsync(x => x.Id, x => x.Name, ct);
        }
        finally { Gate.Release(); }
    }
}
