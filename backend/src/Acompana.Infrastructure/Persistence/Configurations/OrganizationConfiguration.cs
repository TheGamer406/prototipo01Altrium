/*
 * Archivo:    backend/src/Acompana.Infrastructure/Persistence/Configurations/OrganizationConfiguration
 *               .cs
 * Qué hace:   Mapeo EF Core de las tablas: branches, employees, deceased, family_contacts, insurers.
 * Recibe:     ModelBuilder (lo llama AppDbContext).
 * Procesa:    Nombres snake_case, enums como texto, decimales numeric(14,2), índice por national_id.
 * Devuelve:   —
 * Se conecta: ← AppDbContext.OnModelCreating · → migraciones · → db/schema.sql
 * Estado:     Completo
 */

using Acompana.Domain.Organization;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Acompana.Infrastructure.Persistence.Configurations;

public class BranchConfiguration : IEntityTypeConfiguration<Branch>
{
    public void Configure(EntityTypeBuilder<Branch> b)
    {
        b.HasIndex(x => x.Code).IsUnique();
        b.Property(x => x.Code).HasMaxLength(20);
        b.Property(x => x.Name).HasMaxLength(120);
    }
}

public class EmployeeConfiguration : IEntityTypeConfiguration<Employee>
{
    public void Configure(EntityTypeBuilder<Employee> b)
    {
        b.HasOne<Branch>().WithMany().HasForeignKey(x => x.BranchId);
        b.HasIndex(x => x.BranchId);
    }
}

public class DeceasedConfiguration : IEntityTypeConfiguration<Deceased>
{
    public void Configure(EntityTypeBuilder<Deceased> b)
    {
        b.HasIndex(x => x.NationalId);
        b.Property(x => x.NationalId).HasMaxLength(20);
    }
}

public class FamilyContactConfiguration : IEntityTypeConfiguration<FamilyContact>
{
    public void Configure(EntityTypeBuilder<FamilyContact> b)
    {
        b.HasIndex(x => x.NationalId);
        b.Property(x => x.NationalId).HasMaxLength(20);
    }
}

public class InsurerConfiguration : IEntityTypeConfiguration<Insurer>
{
    public void Configure(EntityTypeBuilder<Insurer> b) { }
}
