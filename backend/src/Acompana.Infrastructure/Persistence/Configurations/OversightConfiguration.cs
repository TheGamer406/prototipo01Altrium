/*
 * Archivo:    backend/src/Acompana.Infrastructure/Persistence/Configurations/OversightConfiguration.cs
 * Qué hace:   Mapeo EF Core de las tablas: audit_log, alerts, outbound_messages.
 * Recibe:     ModelBuilder (lo llama AppDbContext).
 * Procesa:    Nombres snake_case, enums como texto, decimales numeric(14,2), jsonb para old/new
 *               values.
 * Devuelve:   —
 * Se conecta: ← AppDbContext.OnModelCreating · → migraciones · → db/schema.sql
 * Estado:     Completo
 */

using Acompana.Domain.Oversight;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Acompana.Infrastructure.Persistence.Configurations;

public class AuditLogConfiguration : IEntityTypeConfiguration<AuditLog>
{
    public void Configure(EntityTypeBuilder<AuditLog> b)
    {
        b.Property(x => x.OldValues).HasColumnType("jsonb");
        b.Property(x => x.NewValues).HasColumnType("jsonb");
        b.HasIndex(x => new { x.Entity, x.EntityId });
        b.HasIndex(x => x.ServiceId);
        b.HasIndex(x => x.At);
    }
}

public class AlertConfiguration : IEntityTypeConfiguration<Alert>
{
    public void Configure(EntityTypeBuilder<Alert> b)
    {
        b.HasOne<Acompana.Domain.Organization.Branch>().WithMany().HasForeignKey(x => x.BranchId);
        b.HasIndex(x => x.AcknowledgedAt);
    }
}

public class OutboundMessageConfiguration : IEntityTypeConfiguration<OutboundMessage>
{
    public void Configure(EntityTypeBuilder<OutboundMessage> b) => b.HasIndex(x => x.ServiceId);
}
