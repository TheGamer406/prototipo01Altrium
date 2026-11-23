/*
 * Archivo:    backend/src/Acompana.Infrastructure/Persistence/AuditInterceptor.cs
 * Qué hace:   Escribe audit_log por cada alta, cambio o baja.
 * Recibe:     SavingChanges de EF Core + ICurrentUser.
 * Procesa:    Recorre ChangeTracker (excepto AuditLog/OutboundMessage/StatusHistory); serializa a
 *               jsonb los valores originales y nuevos de las propiedades modificadas.
 * Devuelve:   Agrega filas AuditLog en el mismo SaveChanges.
 * Se conecta: ← AppDbContext · → AuditQuery (caso de las flores)
 * Reglas:     RF-31 · RNF-13
 * Estado:     Completo
 */

using System.Text.Json;
using System.Text.Json.Serialization;
using Acompana.Application.Common;
using Acompana.Domain.Common;
using Acompana.Domain.Enums;
using Acompana.Domain.Oversight;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Diagnostics;

namespace Acompana.Infrastructure.Persistence;

public sealed class AuditInterceptor : SaveChangesInterceptor
{
    private static readonly JsonSerializerOptions Json = new()
    {
        Converters = { new JsonStringEnumConverter() },
        DefaultIgnoreCondition = JsonIgnoreCondition.Never
    };

    private static readonly HashSet<string> Ignored = new() { "Version", "CreatedAt", "UpdatedAt" };
    private readonly ICurrentUser _user;
    private readonly List<(EntityEntry Entry, AuditLog Log)> _pending = new();
    private bool _inSave;

    /// <summary>El seed y el reinicio la apagan para no auditar miles de filas.</summary>
    public bool Enabled { get; set; } = true;

    public AuditInterceptor(ICurrentUser user) => _user = user;

    public override ValueTask<InterceptionResult<int>> SavingChangesAsync(
        DbContextEventData eventData, InterceptionResult<int> result, CancellationToken ct = default)
    {
        if (Enabled && !_inSave && eventData.Context is { } ctx) Collect(ctx);
        return ValueTask.FromResult(result);
    }

    public override async ValueTask<int> SavedChangesAsync(
        SaveChangesCompletedEventData eventData, int result, CancellationToken ct = default)
    {
        if (_pending.Count > 0 && eventData.Context is { } ctx && !_inSave)
        {
            _inSave = true;
            try
            {
                foreach (var (entry, log) in _pending)
                {
                    if (log.Action == AuditAction.Create)
                        log.EntityId = KeyOf(entry);
                    ctx.Set<AuditLog>().Add(log);
                }
                _pending.Clear();
                await ctx.SaveChangesAsync(ct);
            }
            finally { _inSave = false; }
        }
        return result;
    }

    private void Collect(DbContext ctx)
    {
        _pending.Clear();
        var now = DateTimeOffset.UtcNow;
        foreach (var e in ctx.ChangeTracker.Entries().ToList())
        {
            if (e.Entity is AuditLog or OutboundMessage or Domain.Services.StatusHistory) continue;
            if (e.State is not (EntityState.Added or EntityState.Modified or EntityState.Deleted)) continue;

            var oldV = new Dictionary<string, object?>();
            var newV = new Dictionary<string, object?>();
            foreach (var p in e.Properties)
            {
                var name = p.Metadata.Name;
                if (Ignored.Contains(name) || p.IsTemporary) continue;
                switch (e.State)
                {
                    case EntityState.Added: newV[name] = p.CurrentValue; break;
                    case EntityState.Deleted: oldV[name] = p.OriginalValue; break;
                    case EntityState.Modified when p.IsModified:
                        oldV[name] = p.OriginalValue; newV[name] = p.CurrentValue; break;
                }
            }
            if (e.State == EntityState.Modified && newV.Count == 0) continue;

            // Nombre legible del componente (caso de las flores)
            var comp = e.Entity.GetType().GetProperty("Component")?.GetValue(e.Entity) as Domain.Catalog.Component;
            if (comp is not null)
            {
                var target = e.State == EntityState.Deleted ? oldV : newV;
                target["componentName"] = comp.Name;
            }

            var log = new AuditLog
            {
                At = now,
                ActorRole = _user.Role.ToString(),
                ActorName = _user.DisplayName,
                BranchId = _user.BranchId,
                Entity = e.Metadata.ClrType.Name,
                EntityId = e.State == EntityState.Added ? "" : KeyOf(e),
                Action = e.State switch { EntityState.Added => AuditAction.Create, EntityState.Deleted => AuditAction.Delete, _ => AuditAction.Update },
                OldValues = JsonSerializer.Serialize(oldV, Json),
                NewValues = JsonSerializer.Serialize(newV, Json),
                ServiceId = ServiceIdOf(e)
            };
            _pending.Add((e, log));
        }
    }

    private static string KeyOf(EntityEntry e) =>
        string.Join(",", e.Metadata.FindPrimaryKey()!.Properties.Select(p => e.Property(p.Name).CurrentValue?.ToString()));

    private static Guid? ServiceIdOf(EntityEntry e)
    {
        if (e.Entity is Domain.Services.Service s) return s.Id;
        var prop = e.Metadata.FindProperty("ServiceId");
        return prop is null ? null : e.Property("ServiceId").CurrentValue as Guid?;
    }
}
