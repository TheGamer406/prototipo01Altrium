/*
 * Archivo:    backend/src/Acompana.Application/Services/FamilyNotifier.cs
 * Qué hace:   Arma y envía los correos a la familia (no tiene usuario).
 * Recibe:     Service + tipo de aviso (Registrado, Factura emitida, En curso, Cenizas entregadas).
 * Procesa:    Plantilla en español con código, fecha, lugar y estado; adjunta referencia de factura si
 *               aplica.
 * Devuelve:   Task; queda en outbound_messages (Kind FamilyStatus o FamilyInvoice).
 * Se conecta: ← Create/IssueInvoice/RegisterPayment/AdvanceCremation · → IMailClient
 * Reglas:     RF-35
 * Estado:     Completo
 */

using Acompana.Application.Integrations;
using Acompana.Domain.Billing;
using Acompana.Domain.Enums;
using Acompana.Domain.Services;
using Microsoft.Extensions.Logging;

namespace Acompana.Application.Services;

public sealed class FamilyNotifier
{
    private readonly IMailClient _mail;
    private readonly ILogger<FamilyNotifier> _log;
    public FamilyNotifier(IMailClient mail, ILogger<FamilyNotifier> log) { _mail = mail; _log = log; }

    private static string Local(DateTimeOffset d) => d.ToOffset(TimeSpan.FromHours(-6)).ToString("dd/MM/yyyy HH:mm");

    private async Task Send(Service s, MessageKind kind, string subject, string body, CancellationToken ct)
    {
        try { await _mail.SendAsync(new MailMessage(s.FamilyContact!.Email, subject, body, kind, s.Id), ct); }
        catch (Exception ex) { _log.LogWarning(ex, "No se pudo enviar el correo de {Code}", s.Code); }
    }

    public Task RegisteredAsync(Service s, string branchName, CancellationToken ct) =>
        Send(s, MessageKind.FamilyStatus, $"Servicio {s.Code} registrado",
            $"Estimada familia {s.FamilyContact!.FullName}: su servicio {s.Code} fue registrado en la sede {branchName}. Fecha y hora: {Local(s.ScheduledAt)}. Estado: {(s.Kind == ServiceKind.Advance ? "Anticipado" : "Registrado")}. Estamos para acompañarles.", ct);

    public Task InvoiceAsync(Service s, Invoice inv, CancellationToken ct) =>
        Send(s, MessageKind.FamilyInvoice, $"Factura {inv.CoreReference} del servicio {s.Code}",
            $"Estimada familia: se emitió la factura {inv.CoreReference} por ₡{inv.TotalCrc:N0} (IVA incluido). Receptor: {inv.ReceiverName}.", ct);

    public Task InProgressAsync(Service s, CancellationToken ct) =>
        Send(s, MessageKind.FamilyStatus, $"Servicio {s.Code} en curso",
            $"Estimada familia: el pago fue recibido y su servicio {s.Code} está en curso. Fecha y hora: {Local(s.ScheduledAt)}.", ct);

    public Task AshesDeliveredAsync(Service s, string receivedBy, CancellationToken ct) =>
        Send(s, MessageKind.FamilyStatus, $"Cenizas entregadas - servicio {s.Code}",
            $"Estimada familia: las cenizas fueron entregadas a {receivedBy}. Gracias por permitirnos acompañarles.", ct);
}
