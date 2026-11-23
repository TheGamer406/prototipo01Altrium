/*
 * Archivo:    backend/src/Acompana.Infrastructure/Persistence/AppDbContext.cs
 * Qué hace:   DbContext de EF Core (implementa IAppDbContext).
 * Recibe:     DbContextOptions (Npgsql) + AuditInterceptor.
 * Procesa:    DbSet de todas las entidades; ApplyConfigurationsFromAssembly; extensión btree_gist;
 *               esquema analytics; fija CreatedAt/UpdatedAt en SaveChanges; xmin como token de
 *               concurrencia.
 * Devuelve:   —
 * Se conecta: ← handlers (vía IAppDbContext) · → PostgreSQL acompana-db
 * Reglas:     RF-20 (btree_gist) · RF-31
 * Estado:     Completo
 */

using Acompana.Application.Common;
using Acompana.Domain.Billing;
using Acompana.Domain.Catalog;
using Acompana.Domain.Common;
using Acompana.Domain.Inventory;
using Acompana.Domain.Logistics;
using Acompana.Domain.Organization;
using Acompana.Domain.Oversight;
using Acompana.Domain.Providers;
using Acompana.Domain.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace Acompana.Infrastructure.Persistence;

public class AppDbContext : DbContext, IAppDbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<Branch> Branches => Set<Branch>();
    public DbSet<Employee> Employees => Set<Employee>();
    public DbSet<Deceased> Deceased => Set<Deceased>();
    public DbSet<FamilyContact> FamilyContacts => Set<FamilyContact>();
    public DbSet<Insurer> Insurers => Set<Insurer>();
    public DbSet<Component> Components => Set<Component>();
    public DbSet<Package> Packages => Set<Package>();
    public DbSet<PackageItem> PackageItems => Set<PackageItem>();
    public DbSet<CasketModel> CasketModels => Set<CasketModel>();
    public DbSet<Setting> Settings => Set<Setting>();
    public DbSet<CasketStock> CasketStock => Set<CasketStock>();
    public DbSet<RestockRequest> RestockRequests => Set<RestockRequest>();
    public DbSet<Chapel> Chapels => Set<Chapel>();
    public DbSet<ChapelBooking> ChapelBookings => Set<ChapelBooking>();
    public DbSet<StaffAssignment> StaffAssignments => Set<StaffAssignment>();
    public DbSet<Hearse> Hearses => Set<Hearse>();
    public DbSet<Transfer> Transfers => Set<Transfer>();
    public DbSet<TransferMilestone> TransferMilestones => Set<TransferMilestone>();
    public DbSet<Cremation> Cremations => Set<Cremation>();
    public DbSet<Service> Services => Set<Service>();
    public DbSet<ServiceLine> ServiceLines => Set<ServiceLine>();
    public DbSet<DeathCertificate> DeathCertificates => Set<DeathCertificate>();
    public DbSet<StatusHistory> StatusHistory => Set<StatusHistory>();
    public DbSet<Invoice> Invoices => Set<Invoice>();
    public DbSet<Payment> Payments => Set<Payment>();
    public DbSet<Provider> Providers => Set<Provider>();
    public DbSet<ProviderOffer> ProviderOffers => Set<ProviderOffer>();
    public DbSet<Alert> Alerts => Set<Alert>();
    public DbSet<AuditLog> AuditLog => Set<AuditLog>();
    public DbSet<OutboundMessage> OutboundMessages => Set<OutboundMessage>();

    protected override void ConfigureConventions(ModelConfigurationBuilder b)
    {
        b.Properties<decimal>().HavePrecision(14, 2);
        // Npgsql solo acepta DateTimeOffset con desfase 0 en timestamptz
        b.Properties<DateTimeOffset>().HaveConversion<UtcOffsetConverter>();
        b.Properties<DateTimeOffset?>().HaveConversion<UtcOffsetNullableConverter>();
    }

    protected override void OnModelCreating(ModelBuilder mb)
    {
        mb.HasPostgresExtension("btree_gist");
        mb.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly);

        foreach (var entity in mb.Model.GetEntityTypes())
        {
            foreach (var fk in entity.GetForeignKeys())
                if (fk.DeleteBehavior == DeleteBehavior.ClientSetNull || fk.DeleteBehavior == DeleteBehavior.Cascade && !fk.IsOwnership && fk.PrincipalToDependent is null)
                    fk.DeleteBehavior = DeleteBehavior.Restrict;

            foreach (var p in entity.GetProperties())
            {
                var type = Nullable.GetUnderlyingType(p.ClrType) ?? p.ClrType;
                if (type.IsEnum)
                {
                    var conv = (ValueConverter)Activator.CreateInstance(typeof(EnumToStringConverter<>).MakeGenericType(type))!;
                    p.SetValueConverter(conv);
                    p.SetMaxLength(40);
                }
                if (p.Name == "Id" && p.ClrType == typeof(Guid))
                    p.ValueGenerated = Microsoft.EntityFrameworkCore.Metadata.ValueGenerated.Never; // Guid se asigna en la app: evita que EF crea 'Modified' a filas nuevas
                if (p.Name == "Version" && p.ClrType == typeof(uint))
                {
                    p.SetColumnName("xmin");
                    p.SetColumnType("xid");
                    p.ValueGenerated = Microsoft.EntityFrameworkCore.Metadata.ValueGenerated.OnAddOrUpdate;
                    p.IsConcurrencyToken = true;
                }
            }
        }
    }

    public override Task<int> SaveChangesAsync(CancellationToken ct = default)
    {
        var now = DateTimeOffset.UtcNow;
        foreach (var e in ChangeTracker.Entries<EntityBase>())
        {
            if (e.State == EntityState.Added)
            {
                if (e.Entity.CreatedAt == default) e.Entity.CreatedAt = now;
                e.Entity.UpdatedAt = now;
            }
            else if (e.State == EntityState.Modified) e.Entity.UpdatedAt = now;
        }
        return base.SaveChangesAsync(ct);
    }

    public Task<IDbContextTransaction> BeginTransactionAsync(CancellationToken ct = default) =>
        Database.BeginTransactionAsync(ct);

    public Task<int> ExecuteSqlAsync(FormattableString sql, CancellationToken ct = default) =>
        Database.ExecuteSqlAsync(sql, ct);

    public Task<int> ExecuteSqlRawAsync(string sql, CancellationToken ct = default) =>
        Database.ExecuteSqlRawAsync(sql, ct);

    public IQueryable<T> SqlQueryRaw<T>(string sql, params object[] args) =>
        Database.SqlQueryRaw<T>(sql, args);
}

public class UtcOffsetConverter : ValueConverter<DateTimeOffset, DateTimeOffset>
{
    public UtcOffsetConverter() : base(v => v.ToUniversalTime(), v => v) { }
}

public class UtcOffsetNullableConverter : ValueConverter<DateTimeOffset?, DateTimeOffset?>
{
    public UtcOffsetNullableConverter() : base(v => v.HasValue ? v.Value.ToUniversalTime() : v, v => v) { }
}
