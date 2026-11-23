/*
 * Archivo:    backend/src/Acompana.Application/DependencyInjection.cs
 * Qué hace:   Registra en el contenedor todos los handlers, queries y servicios de Application.
 * Recibe:     IServiceCollection desde Program.cs.
 * Procesa:    AddScoped de cada handler/query; AddSingleton de InvoiceCalculator.
 * Devuelve:   La misma IServiceCollection (encadenable).
 * Se conecta: ← Api/Program.cs (AddApplication())
 * Estado:     Completo
 */

using Acompana.Application.Admin;
using Acompana.Application.Analytics;
using Acompana.Application.Billing;
using Acompana.Application.Catalog;
using Acompana.Application.Inventory;
using Acompana.Application.Logistics;
using Acompana.Application.Meta;
using Acompana.Application.Oversight;
using Acompana.Application.Providers;
using Acompana.Application.Services;
using Microsoft.Extensions.DependencyInjection;

namespace Acompana.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection s)
    {
        s.AddSingleton<InvoiceCalculator>();
        // Servicios
        s.AddScoped<GetServiceDetailQuery>().AddScoped<GetServicesQuery>().AddScoped<GetServiceBoardQuery>().AddScoped<GetServiceTimelineQuery>()
            .AddScoped<CreateServiceHandler>().AddScoped<UpdateServiceHandler>().AddScoped<ActivateAdvanceServiceHandler>()
            .AddScoped<SetServicePackageHandler>().AddScoped<RegisterDeathCertificateHandler>().AddScoped<ExportProspectsQuery>()
            .AddScoped<FamilyNotifier>();
        // Facturación
        s.AddScoped<IssueInvoiceHandler>().AddScoped<RegisterPaymentHandler>().AddScoped<GetInvoiceQuery>();
        // Proveedores
        s.AddScoped<DispatchOffersHandler>().AddScoped<AcceptOfferHandler>().AddScoped<GetProviderInboxQuery>()
            .AddScoped<GetOpenOpportunitiesQuery>().AddScoped<OfferTimeoutChecker>().AddScoped<ProvidersHandler>();
        // Inventario y logística
        s.AddScoped<DeductCasketHandler>().AddScoped<StockQuery>().AddScoped<RestockRequestsHandler>()
            .AddScoped<BookChapelHandler>().AddScoped<ChapelCalendarQuery>().AddScoped<AssignStaffHandler>().AddScoped<AdvanceCremationHandler>().AddScoped<CremationQueueQuery>();
        // Supervisión y analítica
        s.AddScoped<AlertService>().AddScoped<AlertsHandler>().AddScoped<MessagesQuery>().AddScoped<AuditQuery>()
            .AddScoped<OverviewQuery>().AddScoped<SettingsHandler>();
        s.AddScoped<SalesQuery>().AddScoped<StageTimesQuery>().AddScoped<ZonesQuery>();
        // Catálogo, meta y administración
        s.AddScoped<ComponentsHandler>().AddScoped<PackagesHandler>().AddScoped<CasketModelsHandler>().AddScoped<MetaQuery>();
        s.AddScoped<ResetDemoHandler>().AddScoped<RefreshAnalyticsHandler>().AddScoped<SimulationHandler>();
        return s;
    }
}
