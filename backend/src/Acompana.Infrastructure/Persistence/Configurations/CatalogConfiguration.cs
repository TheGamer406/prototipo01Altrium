/*
 * Archivo:    backend/src/Acompana.Infrastructure/Persistence/Configurations/CatalogConfiguration.cs
 * Qué hace:   Mapeo EF Core de las tablas: components, packages, package_items, casket_models,
 *               settings.
 * Recibe:     ModelBuilder (lo llama AppDbContext).
 * Procesa:    Nombres snake_case, enums como texto, decimales numeric(14,2), sku único.
 * Devuelve:   —
 * Se conecta: ← AppDbContext.OnModelCreating · → migraciones · → db/schema.sql
 * Estado:     Completo
 */

using Acompana.Domain.Catalog;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Acompana.Infrastructure.Persistence.Configurations;

public class ComponentConfiguration : IEntityTypeConfiguration<Component>
{
    public void Configure(EntityTypeBuilder<Component> b) => b.Property(x => x.Name).HasMaxLength(120);
}

public class PackageConfiguration : IEntityTypeConfiguration<Package>
{
    public void Configure(EntityTypeBuilder<Package> b)
    {
        b.Ignore(x => x.PriceCrc);
        b.HasMany(x => x.Items).WithOne().HasForeignKey(i => i.PackageId).OnDelete(DeleteBehavior.Cascade);
    }
}

public class PackageItemConfiguration : IEntityTypeConfiguration<PackageItem>
{
    public void Configure(EntityTypeBuilder<PackageItem> b) =>
        b.HasOne(x => x.Component).WithMany().HasForeignKey(x => x.ComponentId);
}

public class CasketModelConfiguration : IEntityTypeConfiguration<CasketModel>
{
    public void Configure(EntityTypeBuilder<CasketModel> b) => b.HasIndex(x => x.Sku).IsUnique();
}

public class SettingConfiguration : IEntityTypeConfiguration<Setting>
{
    public void Configure(EntityTypeBuilder<Setting> b) => b.HasIndex(x => x.Key).IsUnique();
}
