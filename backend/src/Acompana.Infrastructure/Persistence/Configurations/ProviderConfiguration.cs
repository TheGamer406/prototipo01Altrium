/*
 * Archivo:    backend/src/Acompana.Infrastructure/Persistence/Configurations/ProviderConfiguration.cs
 * Qué hace:   Mapeo EF Core de las tablas: providers, provider_offers.
 * Recibe:     ModelBuilder (lo llama AppDbContext).
 * Procesa:    Nombres snake_case, enums como texto, decimales numeric(14,2), índice (service_line_id,
 *               status).
 * Devuelve:   —
 * Se conecta: ← AppDbContext.OnModelCreating · → migraciones · → db/schema.sql
 * Estado:     Completo
 */

using Acompana.Domain.Providers;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Acompana.Infrastructure.Persistence.Configurations;

public class ProviderEntityConfiguration : IEntityTypeConfiguration<Provider>
{
    public void Configure(EntityTypeBuilder<Provider> b) => b.HasIndex(x => new { x.Type, x.Zone });
}

public class ProviderOfferConfiguration : IEntityTypeConfiguration<ProviderOffer>
{
    public void Configure(EntityTypeBuilder<ProviderOffer> b)
    {
        b.HasOne(x => x.ServiceLine).WithMany().HasForeignKey(x => x.ServiceLineId);
        b.HasOne(x => x.Provider).WithMany().HasForeignKey(x => x.ProviderId);
        b.HasIndex(x => new { x.ServiceLineId, x.Status });
        b.HasIndex(x => new { x.ProviderId, x.Status });
    }
}
