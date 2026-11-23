/*
 * Archivo:    backend/src/Acompana.Infrastructure/Persistence/Configurations/ServiceConfiguration.cs
 * Qué hace:   Mapeo EF Core de las tablas: services, service_lines, death_certificates,
 *               status_history.
 * Recibe:     ModelBuilder (lo llama AppDbContext).
 * Procesa:    Nombres snake_case, enums como texto, decimales numeric(14,2), code único;
 *               death_certificates.service_id único; índices (branch_id, status).
 * Devuelve:   —
 * Se conecta: ← AppDbContext.OnModelCreating · → migraciones · → db/schema.sql
 * Estado:     Completo
 */

using Acompana.Domain.Catalog;
using Acompana.Domain.Organization;
using Acompana.Domain.Providers;
using Acompana.Domain.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Acompana.Infrastructure.Persistence.Configurations;

public class ServiceEntityConfiguration : IEntityTypeConfiguration<Service>
{
    public void Configure(EntityTypeBuilder<Service> b)
    {
        b.HasIndex(x => x.Code).IsUnique();
        b.Property(x => x.Code).HasMaxLength(20);
        b.HasIndex(x => new { x.BranchId, x.Status });
        b.HasIndex(x => x.ScheduledAt);
        b.HasOne(x => x.Branch).WithMany().HasForeignKey(x => x.BranchId);
        b.HasOne(x => x.Deceased).WithMany().HasForeignKey(x => x.DeceasedId);
        b.HasOne(x => x.FamilyContact).WithMany().HasForeignKey(x => x.FamilyContactId);
        b.HasOne(x => x.BasePackage).WithMany().HasForeignKey(x => x.BasePackageId);
        b.HasOne(x => x.Insurer).WithMany().HasForeignKey(x => x.InsurerId);
        b.HasMany(x => x.Lines).WithOne().HasForeignKey(l => l.ServiceId).OnDelete(DeleteBehavior.Cascade);
        b.HasMany(x => x.History).WithOne().HasForeignKey(h => h.ServiceId).OnDelete(DeleteBehavior.Cascade);
        b.HasOne(x => x.DeathCertificate).WithOne().HasForeignKey<DeathCertificate>(d => d.ServiceId).OnDelete(DeleteBehavior.Cascade);
        b.HasMany(x => x.Invoices).WithOne().HasForeignKey(i => i.ServiceId).OnDelete(DeleteBehavior.Cascade);
    }
}

public class ServiceLineConfiguration : IEntityTypeConfiguration<ServiceLine>
{
    public void Configure(EntityTypeBuilder<ServiceLine> b)
    {
        b.HasOne(x => x.Component).WithMany().HasForeignKey(x => x.ComponentId);
        b.HasOne(x => x.CasketModel).WithMany().HasForeignKey(x => x.CasketModelId);
        b.HasOne<Provider>().WithMany().HasForeignKey(x => x.ProviderId);
        b.HasIndex(x => x.Status);
    }
}

public class DeathCertificateConfiguration : IEntityTypeConfiguration<DeathCertificate>
{
    public void Configure(EntityTypeBuilder<DeathCertificate> b) => b.HasIndex(x => x.ServiceId).IsUnique();
}

public class StatusHistoryConfiguration : IEntityTypeConfiguration<StatusHistory>
{
    public void Configure(EntityTypeBuilder<StatusHistory> b) => b.HasIndex(x => new { x.ServiceId, x.ChangedAt });
}
