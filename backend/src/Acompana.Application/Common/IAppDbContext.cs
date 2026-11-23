/*
 * Archivo:    backend/src/Acompana.Application/Common/IAppDbContext.cs
 * Qué hace:   Interfaz del contexto de BD que usan los handlers (lo implementa Infrastructure).
 * Recibe:     —
 * Procesa:    DbSet<> de todas las entidades + SaveChangesAsync + BeginTransactionAsync +
 *               ExecuteSqlAsync (para la adjudicación y el refresco de vistas).
 * Devuelve:   —
 * Se conecta: ← handlers · → Infrastructure/Persistence/AppDbContext
 * Estado:     Completo
 */

using Acompana.Domain.Billing;
using Acompana.Domain.Catalog;
using Acompana.Domain.Inventory;
using Acompana.Domain.Logistics;
using Acompana.Domain.Organization;
using Acompana.Domain.Oversight;
using Acompana.Domain.Providers;
using Acompana.Domain.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;

namespace Acompana.Application.Common;

public interface IAppDbContext
{
    DbSet<Branch> Branches { get; }
    DbSet<Employee> Employees { get; }
    DbSet<Deceased> Deceased { get; }
    DbSet<FamilyContact> FamilyContacts { get; }
    DbSet<Insurer> Insurers { get; }
    DbSet<Component> Components { get; }
    DbSet<Package> Packages { get; }
    DbSet<PackageItem> PackageItems { get; }
    DbSet<CasketModel> CasketModels { get; }
    DbSet<Setting> Settings { get; }
    DbSet<CasketStock> CasketStock { get; }
    DbSet<RestockRequest> RestockRequests { get; }
    DbSet<Chapel> Chapels { get; }
    DbSet<ChapelBooking> ChapelBookings { get; }
    DbSet<StaffAssignment> StaffAssignments { get; }
    DbSet<Hearse> Hearses { get; }
    DbSet<Transfer> Transfers { get; }
    DbSet<TransferMilestone> TransferMilestones { get; }
    DbSet<Cremation> Cremations { get; }
    DbSet<Service> Services { get; }
    DbSet<ServiceLine> ServiceLines { get; }
    DbSet<DeathCertificate> DeathCertificates { get; }
    DbSet<StatusHistory> StatusHistory { get; }
    DbSet<Invoice> Invoices { get; }
    DbSet<Payment> Payments { get; }
    DbSet<Provider> Providers { get; }
    DbSet<ProviderOffer> ProviderOffers { get; }
    DbSet<Alert> Alerts { get; }
    DbSet<AuditLog> AuditLog { get; }
    DbSet<OutboundMessage> OutboundMessages { get; }

    Task<int> SaveChangesAsync(CancellationToken ct = default);
    Task<IDbContextTransaction> BeginTransactionAsync(CancellationToken ct = default);
    Task<int> ExecuteSqlAsync(FormattableString sql, CancellationToken ct = default);
    Task<int> ExecuteSqlRawAsync(string sql, CancellationToken ct = default);
}
