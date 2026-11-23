/*
 * Archivo:    backend/src/Acompana.Infrastructure/Persistence/Configurations/BillingConfiguration.cs
 * Qué hace:   Mapeo EF Core de las tablas: invoices, payments.
 * Recibe:     ModelBuilder (lo llama AppDbContext).
 * Procesa:    Nombres snake_case, enums como texto, decimales numeric(14,2), idempotency_key único.
 * Devuelve:   —
 * Se conecta: ← AppDbContext.OnModelCreating · → migraciones · → db/schema.sql
 * Estado:     Completo
 */

using Acompana.Domain.Billing;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Acompana.Infrastructure.Persistence.Configurations;

public class InvoiceConfiguration : IEntityTypeConfiguration<Invoice>
{
    public void Configure(EntityTypeBuilder<Invoice> b)
    {
        b.HasIndex(x => x.IdempotencyKey).IsUnique();
        b.Property(x => x.IdempotencyKey).HasMaxLength(80);
        b.HasMany(x => x.Payments).WithOne().HasForeignKey(p => p.InvoiceId).OnDelete(DeleteBehavior.Cascade);
        // A lo sumo una factura aceptada por servicio (RF-13)
        b.HasIndex(x => x.ServiceId).HasFilter("status = 'Accepted'").IsUnique().HasDatabaseName("ux_invoices_service_accepted");
    }
}

public class PaymentConfiguration : IEntityTypeConfiguration<Payment>
{
    public void Configure(EntityTypeBuilder<Payment> b) { }
}
