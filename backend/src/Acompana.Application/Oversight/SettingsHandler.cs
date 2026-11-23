/*
 * Archivo:    backend/src/Acompana.Application/Oversight/SettingsHandler.cs
 * Qué hace:   Lee y guarda parámetros configurables.
 * Recibe:     — · { key: value }.
 * Procesa:    Solo Management escribe; valida enteros positivos.
 * Devuelve:   Dictionary<string,string>.
 * Se conecta: ← GET/PUT /api/settings · pantalla Parámetros
 * Reglas:     RF-17 · RF-24
 * Estado:     Completo
 */

using Acompana.Application.Common;
using Acompana.Domain.Catalog;
using Acompana.Domain.Enums;
using Microsoft.EntityFrameworkCore;

namespace Acompana.Application.Oversight;

public sealed class SettingsHandler
{
    private readonly IAppDbContext _db;
    private readonly ICurrentUser _user;
    public SettingsHandler(IAppDbContext db, ICurrentUser user) { _db = db; _user = user; }

    public async Task<Dictionary<string, string>> GetAsync(CancellationToken ct) =>
        await _db.Settings.AsNoTracking().ToDictionaryAsync(s => s.Key, s => s.Value, ct);

    public async Task<int> GetIntAsync(string key, int fallback, CancellationToken ct)
    {
        var v = await _db.Settings.AsNoTracking().Where(s => s.Key == key).Select(s => s.Value).FirstOrDefaultAsync(ct);
        return int.TryParse(v, out var n) ? n : fallback;
    }

    public async Task<Dictionary<string, string>> PutAsync(Dictionary<string, string> values, CancellationToken ct)
    {
        if (_user.Role != DemoRole.Management) throw new ForbiddenException("Solo Gerencia puede cambiar los parámetros.");
        var allowed = new[] { Setting.ProviderAcceptTimeoutMin, Setting.TransferThresholdMin, Setting.DefaultMinStock };
        foreach (var (key, value) in values)
        {
            if (!allowed.Contains(key)) throw new ValidationException(key, "Parámetro desconocido.");
            if (!int.TryParse(value, out var n) || n < 1) throw new ValidationException(key, "Ingrese un número entero positivo.");
            var row = await _db.Settings.FirstOrDefaultAsync(s => s.Key == key, ct);
            if (row is null) _db.Settings.Add(new Setting { Key = key, Value = n.ToString() });
            else row.Value = n.ToString();
        }
        await _db.SaveChangesAsync(ct);
        return await GetAsync(ct);
    }
}
