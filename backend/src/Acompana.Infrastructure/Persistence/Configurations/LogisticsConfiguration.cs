/*
 * Archivo:    backend/src/Acompana.Infrastructure/Persistence/Configurations/LogisticsConfiguration.cs
 * Qué hace:   Mapeo EF Core de las tablas: chapels, chapel_bookings, staff_assignments, hearses,
 *               transfers, transfer_milestones, cremations.
 * Recibe:     ModelBuilder (lo llama AppDbContext).
 * Procesa:    Nombres snake_case, enums como texto, decimales numeric(14,2), EXCLUDE USING gist
 *               (chapel_id WITH =, period WITH &&) vía migración SQL; client_uuid único.
 * Devuelve:   —
 * Se conecta: ← AppDbContext.OnModelCreating · → migraciones · → db/schema.sql
 * Estado:     Completo
 */

using Acompana.Domain.Logistics;
using Acompana.Domain.Organization;
using Acompana.Domain.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Acompana.Infrastructure.Persistence.Configurations;

public class ChapelConfiguration : IEntityTypeConfiguration<Chapel>
{
    public void Configure(EntityTypeBuilder<Chapel> b) => b.HasOne<Branch>().WithMany().HasForeignKey(x => x.BranchId);
}

public class ChapelBookingConfiguration : IEntityTypeConfiguration<ChapelBooking>
{
    public void Configure(EntityTypeBuilder<ChapelBooking> b)
    {
        b.HasOne(x => x.Chapel).WithMany().HasForeignKey(x => x.ChapelId);
        b.HasOne<Service>().WithMany().HasForeignKey(x => x.ServiceId);
        b.HasIndex(x => x.ServiceId);
        // EXCLUDE USING gist (chapel_id WITH =, tstzrange(starts_at, ends_at) WITH &&) se crea en la migración inicial.
    }
}

public class StaffAssignmentConfiguration : IEntityTypeConfiguration<StaffAssignment>
{
    public void Configure(EntityTypeBuilder<StaffAssignment> b)
    {
        b.HasOne<Service>().WithMany().HasForeignKey(x => x.ServiceId);
        b.HasOne<Employee>().WithMany().HasForeignKey(x => x.EmployeeId);
    }
}

public class HearseConfiguration : IEntityTypeConfiguration<Hearse>
{
    public void Configure(EntityTypeBuilder<Hearse> b) => b.HasOne<Branch>().WithMany().HasForeignKey(x => x.BranchId);
}

public class TransferConfiguration : IEntityTypeConfiguration<Transfer>
{
    public void Configure(EntityTypeBuilder<Transfer> b)
    {
        b.HasOne<Service>().WithMany().HasForeignKey(x => x.ServiceId);
        b.HasOne<Hearse>().WithMany().HasForeignKey(x => x.HearseId);
        b.HasOne<Employee>().WithMany().HasForeignKey(x => x.DriverId);
        b.HasMany(x => x.Milestones).WithOne().HasForeignKey(m => m.TransferId).OnDelete(DeleteBehavior.Cascade);
    }
}

public class TransferMilestoneConfiguration : IEntityTypeConfiguration<TransferMilestone>
{
    public void Configure(EntityTypeBuilder<TransferMilestone> b) => b.HasIndex(x => x.ClientUuid).IsUnique();
}

public class CremationConfiguration : IEntityTypeConfiguration<Cremation>
{
    public void Configure(EntityTypeBuilder<Cremation> b)
    {
        b.HasOne<Service>().WithMany().HasForeignKey(x => x.ServiceId);
        b.HasOne<Employee>().WithMany().HasForeignKey(x => x.OperatorId);
    }
}
