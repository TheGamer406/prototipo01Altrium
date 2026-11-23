/*
 * Archivo:    backend/src/Acompana.Application/Services/RegisterDeathCertificateHandler.cs
 * Qué hace:   Registra el acta de defunción (número + archivo) del servicio.
 * Recibe:     id + multipart { number, file (PDF/JPG/PNG ≤ 5 MB) }.
 * Procesa:    Guarda el archivo en el volumen /data/uploads/{serviceId}/; crea o reemplaza
 *               DeathCertificate.
 * Devuelve:   DeathCertificateDto { number, fileUrl, registeredBy, registeredAt }.
 * Se conecta: ← POST /api/services/{id}/death-certificate · → habilita Invoicing
 *               (Service.ChangeStatus)
 * Reglas:     RF-03
 * Estado:     Completo
 *
 * Contrato:
 *   Task<DeathCertificateDto> HandleAsync(Guid id, string number, IFormFile file, CancellationToken
 *       ct)
 *     Recibe:     id, número de acta, archivo
 *     Procesa:    ver arriba
 *     Devuelve:   datos del acta
 *     Errores:    400 'Formato no permitido (PDF, JPG o PNG).' · 400 'Archivo mayor a 5 MB.'
 */

using Acompana.Application.Common;
using Acompana.Domain.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace Acompana.Application.Services;

public sealed class RegisterDeathCertificateHandler
{
    private static readonly string[] Allowed = { ".pdf", ".jpg", ".jpeg", ".png" };
    private const long MaxBytes = 5 * 1024 * 1024;

    private readonly IAppDbContext _db;
    private readonly ICurrentUser _user;
    private readonly IClock _clock;
    private readonly IOpsNotifier _notifier;
    private readonly IConfiguration _config;

    public RegisterDeathCertificateHandler(IAppDbContext db, ICurrentUser user, IClock clock, IOpsNotifier notifier, IConfiguration config)
    { _db = db; _user = user; _clock = clock; _notifier = notifier; _config = config; }

    public static string UploadsRoot(IConfiguration config)
    {
        var root = config["UploadsPath"] ?? "/data/uploads";
        try { Directory.CreateDirectory(root); return root; }
        catch { var tmp = Path.Combine(Path.GetTempPath(), "acompana-uploads"); Directory.CreateDirectory(tmp); return tmp; }
    }

    public async Task<DeathCertificateDto> HandleAsync(Guid id, string number, IFormFile file, CancellationToken ct)
    {
        var s = await _db.Services.Include(x => x.DeathCertificate).FirstOrDefaultAsync(x => x.Id == id, ct);
        if (s is null || (!_user.SeesAllBranches && s.BranchId != _user.BranchId)) throw new NotFoundException("El servicio no existe.");
        if (string.IsNullOrWhiteSpace(number)) throw new ValidationException("number", "Indique el número de acta.");
        var ext = Path.GetExtension(file.FileName).ToLowerInvariant();
        if (!Allowed.Contains(ext)) throw new ValidationException("file", "Formato no permitido (PDF, JPG o PNG).");
        if (file.Length > MaxBytes) throw new ValidationException("file", "Archivo mayor a 5 MB.");

        var dir = Path.Combine(UploadsRoot(_config), id.ToString());
        Directory.CreateDirectory(dir);
        foreach (var old in Directory.GetFiles(dir, "acta.*")) File.Delete(old);
        var path = Path.Combine(dir, $"acta{ext}");
        await using (var fs = File.Create(path)) await file.CopyToAsync(fs, ct);

        if (s.DeathCertificate is null)
            s.DeathCertificate = new DeathCertificate { ServiceId = id, Number = number.Trim(), FilePath = path, RegisteredBy = _user.DisplayName, RegisteredAt = _clock.Now };
        else
        {
            s.DeathCertificate.Number = number.Trim(); s.DeathCertificate.FilePath = path;
            s.DeathCertificate.RegisteredBy = _user.DisplayName; s.DeathCertificate.RegisteredAt = _clock.Now;
        }
        await _db.SaveChangesAsync(ct);
        await _notifier.ServiceUpdated(id, s.BranchId);
        var c = s.DeathCertificate!;
        return new DeathCertificateDto(c.Number, $"/api/services/{id}/death-certificate/file", c.RegisteredBy, c.RegisteredAt);
    }
}
