/*
 * Archivo:    backend/src/Acompana.Application/Meta/MetaQuery.cs
 * Qué hace:   Datos para selectores de la interfaz.
 * Recibe:     nada.
 * Procesa:    Sedes (id, code, name, zone), roles demo con etiqueta en español, enums con traducción.
 * Devuelve:   { branches[], roles[{value, label}], enums{name: [{value, label}]} }.
 * Se conecta: ← GET /api/meta/* · → selectores de sede y rol
 * Reglas:     RF-30
 * Estado:     Completo
 */

using Acompana.Application.Common;
using Microsoft.EntityFrameworkCore;

namespace Acompana.Application.Meta;

public record BranchMetaDto(long Id, string Code, string Name, string Zone);
public record OptionDto(string Value, string Label);
public record ProviderMetaDto(long Id, string Name, string Type, string Zone);
public record MetaDto(IReadOnlyList<BranchMetaDto> Branches, IReadOnlyList<OptionDto> Roles, IReadOnlyDictionary<string, IReadOnlyList<OptionDto>> Enums);

public sealed class MetaQuery
{
    private readonly IAppDbContext _db;
    public MetaQuery(IAppDbContext db) => _db = db;

    public static readonly IReadOnlyList<OptionDto> Roles = new OptionDto[]
    {
        new("Advisor", "Asesor funerario"), new("BranchManager", "Encargado de sede"), new("CrematoriumOperator", "Operador de crematorio"),
        new("Provider", "Proveedor"), new("Management", "Gerencia"), new("Driver", "Chofer"),
    };

    private static IReadOnlyList<OptionDto> O(params (string, string)[] items) => items.Select(i => new OptionDto(i.Item1, i.Item2)).ToList();

    public static readonly IReadOnlyDictionary<string, IReadOnlyList<OptionDto>> Enums = new Dictionary<string, IReadOnlyList<OptionDto>>
    {
        ["ServiceKind"] = O(("Immediate", "Inmediato"), ("Advance", "Anticipado")),
        ["ServiceStatus"] = O(("Advance", "Anticipado"), ("Registered", "Registrado"), ("Invoicing", "En facturación"), ("Paid", "Pagado"), ("InProgress", "En curso"), ("Completed", "Finalizado"), ("Cancelled", "Cancelado")),
        ["WakePlace"] = O(("Chapel", "Capilla"), ("Church", "Iglesia"), ("Home", "Domicilio")),
        ["FinalDestination"] = O(("Cemetery", "Cementerio"), ("Cremation", "Cremación")),
        ["Payer"] = O(("Family", "Familia"), ("Insurer", "Aseguradora")),
        ["LineStatus"] = O(("Pending", "Pendiente"), ("Offered", "Ofertado"), ("Accepted", "Aceptado"), ("Delivered", "Entregado")),
        ["InvoiceStatus"] = O(("Draft", "Borrador"), ("Sent", "Enviada"), ("Accepted", "Aceptada"), ("Rejected", "Rechazada")),
        ["PaymentMethod"] = O(("Card", "Tarjeta"), ("Transfer", "Transferencia"), ("Cash", "Efectivo"), ("Insurer", "Aseguradora")),
        ["ComponentCategory"] = O(("Casket", "Cofre"), ("Flowers", "Flores"), ("Hearse", "Carroza"), ("Chapel", "Capilla"), ("Church", "Iglesia"), ("BodyPreparation", "Preparación del cuerpo"), ("Cremation", "Cremación"), ("Catering", "Catering"), ("Music", "Música"), ("Extra", "Adicional")),
        ["ProviderType"] = O(("Flowers", "Floristería"), ("Catering", "Catering"), ("Music", "Música")),
        ["CasketOrigin"] = O(("Nacional", "Nacional"), ("Importado", "Importado")),
        ["Zone"] = O(("GAM", "GAM"), ("Fuera", "Fuera del GAM")),
        ["OfferStatus"] = O(("Open", "Abierta"), ("Won", "Ganada"), ("Lost", "Perdida"), ("Expired", "Vencida")),
        ["AlertType"] = O(("ProviderTimeout", "Proveedor sin responder"), ("LowStock", "Inventario bajo"), ("TransferDelay", "Traslado demorado"), ("StaffOverlap", "Traslape de personal"), ("InvoiceRejected", "Factura rechazada")),
        ["MessageKind"] = O(("FamilyStatus", "Estado a la familia"), ("FamilyInvoice", "Factura a la familia"), ("InsurerNotice", "Aviso a aseguradora"), ("ProviderOffer", "Oferta a proveedor"), ("Alert", "Alerta")),
        ["RestockStatus"] = O(("Requested", "Solicitado"), ("Dispatched", "Despachado"), ("Received", "Recibido")),
        ["CremationStatus"] = O(("Waiting", "En espera"), ("InOven", "En horno"), ("Packed", "Empacado"), ("Delivered", "Entregado")),
        ["TransferStatus"] = O(("Planned", "Planificado"), ("OnRoute", "En ruta"), ("Done", "Finalizado")),
        ["MilestoneType"] = O(("Departure", "Salida"), ("Pickup", "Recogida"), ("ArrivalWake", "Llegada a velación"), ("DepartureWake", "Salida de velación"), ("ArrivalDestination", "Llegada al destino")),
        ["AuditAction"] = O(("Create", "Creó"), ("Update", "Modificó"), ("Delete", "Eliminó")),
    };

    public async Task<IReadOnlyList<BranchMetaDto>> BranchesAsync(CancellationToken ct) =>
        (await _db.Branches.AsNoTracking().OrderBy(b => b.Name).ToListAsync(ct)).Select(b => new BranchMetaDto(b.Id, b.Code, b.Name, b.Zone.ToString())).ToList();

    public async Task<IReadOnlyList<ProviderMetaDto>> ProvidersAsync(CancellationToken ct) =>
        (await _db.Providers.AsNoTracking().OrderBy(p => p.Name).ToListAsync(ct)).Select(p => new ProviderMetaDto(p.Id, p.Name, p.Type.ToString(), p.Zone.ToString())).ToList();

    public async Task<IReadOnlyList<OptionDto>> InsurersAsync(CancellationToken ct) =>
        (await _db.Insurers.AsNoTracking().OrderBy(i => i.Name).ToListAsync(ct)).Select(i => new OptionDto(i.Id.ToString(), i.Name)).ToList();
}
