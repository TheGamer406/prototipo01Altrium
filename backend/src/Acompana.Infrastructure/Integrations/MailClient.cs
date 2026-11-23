/*
 * Archivo:    backend/src/Acompana.Infrastructure/Integrations/MailClient.cs
 * Qué hace:   Implementa IMailClient con HttpClient tipado.
 * Recibe:     DTO de Application + base URL del servicio simulado (MockBaseUrl = http://integrations-
 *               mock:8081).
 * Procesa:    POST /mail/send + insert en outbound_messages; serializa JSON; reintentos según
 *               ResiliencePolicies.
 * Devuelve:   El resultado definido en IMailClient.
 * Se conecta: ← Application/Integrations/IMailClient · → Acompana.Integrations.Mock
 * Reglas:     RNF-15
 * Estado:     Completo
 */

using System.Net.Http.Json;
using Acompana.Application.Common;
using Acompana.Application.Integrations;
using Acompana.Domain.Oversight;

namespace Acompana.Infrastructure.Integrations;

/// <summary>
/// Guarda siempre la copia en outbound_messages; el envío al servidor simulado no bloquea
/// (un correo no debe frenar la operación).
/// </summary>
public sealed class MailClient : IMailClient
{
    private readonly HttpClient _http;
    private readonly IAppDbContext _db;
    public MailClient(HttpClient http, IAppDbContext db) { _http = http; _db = db; }

    public async Task SendAsync(MailMessage msg, CancellationToken ct)
    {
        _db.OutboundMessages.Add(new OutboundMessage
        {
            ToAddress = msg.To, Kind = msg.Kind, Subject = msg.Subject, Body = msg.Body, ServiceId = msg.ServiceId,
        });
        await _db.SaveChangesAsync(ct);
        _ = Task.Run(async () =>
        {
            try { using var _ = await _http.PostAsJsonAsync("/mail/send", new { to = msg.To, subject = msg.Subject, body = msg.Body }); }
            catch { /* el correo simulado es de mejor esfuerzo */ }
        });
    }
}
