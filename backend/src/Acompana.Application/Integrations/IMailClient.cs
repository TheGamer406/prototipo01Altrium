/*
 * Archivo:    backend/src/Acompana.Application/Integrations/IMailClient.cs
 * Qué hace:   Cliente del servidor de correo simulado.
 * Recibe:     —
 * Procesa:    Interfaz; la implementación vive en Infrastructure/Integrations.
 * Devuelve:   —
 * Se conecta: ← handlers · → Infrastructure/Integrations (HttpClient al servicio simulado)
 * Reglas:     RF-12 · RF-15 · RF-35
 * Estado:     Completo
 *
 * Contrato:
 *   Task SendAsync(MailMessage msg, CancellationToken ct)
 *     Recibe:     para, asunto, cuerpo, tipo (MessageKind), serviceId?
 *     Procesa:    POST /mail/send y guarda copia en outbound_messages
 *     Devuelve:   Task
 */

using Acompana.Domain.Enums;

namespace Acompana.Application.Integrations;

public record MailMessage(string To, string Subject, string Body, MessageKind Kind, Guid? ServiceId);

public interface IMailClient
{
    Task SendAsync(MailMessage msg, CancellationToken ct);
}
