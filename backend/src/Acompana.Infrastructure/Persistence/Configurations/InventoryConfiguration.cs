/*
 * Archivo:    backend/src/Acompana.Infrastructure/Persistence/Configurations/InventoryConfiguration.cs
 * Qué hace:   Mapeo EF Core de las tablas: casket_stock, restock_requests.
 * Recibe:     ModelBuilder (lo llama AppDbContext).
 * Procesa:    Nombres snake_case, enums como texto, decimales numeric(14,2), (branch_id,
 *               casket_model_id) único.
 * Devuelve:   —
 * Se conecta: ← AppDbContext.OnModelCreating · → migraciones · → db/schema.sql
 * Estado:     Completo
 */

using Acompana.Domain.Catalog;
using Acompana.Domain.Inventory;
using Acompana.Domain.Organization;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Acompana.Infrastructure.Persistence.Configurations;

public class CasketStockConfiguration : IEntityTypeConfiguration<CasketStock>
{
    public void Configure(EntityTypeBuilder<CasketStock> b)
    {
        b.Ignore(x => x.IsAtOrBelowMin);
        b.HasOne<Branch>().WithMany().HasForeignKey(x => x.BranchId);
        b.HasOne<CasketModel>().WithMany().HasForeignKey(x => x.CasketModelId);
        b.HasIndex(x => new { x.BranchId, x.CasketModelId }).IsUnique();
    }
}

public class RestockRequestConfiguration : IEntityTypeConfiguration<RestockRequest>
{
    public void Configure(EntityTypeBuilder<RestockRequest> b)
    {
        b.HasOne<Branch>().WithMany().HasForeignKey(x => x.BranchId);
        b.HasOne<CasketModel>().WithMany().HasForeignKey(x => x.CasketModelId);
    }
}
