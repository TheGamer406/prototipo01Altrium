/*
 * Archivo:    backend/src/Acompana.Domain/Oversight/OutboundMessage.cs
 * Qué hace:   Entidad OutboundMessage, se guarda en la tabla `outbound_messages`.
 * Recibe:     Campos: Channel, ToAddress, Kind (MessageKind), Subject, Body, ServiceId?, CreatedAt.
 *               Los llena el handler que la crea o EF Core al leer.
 * Procesa:    Solo inserción. Es el registro de los correos simulados.
 * Devuelve:   Nada; es estado. Hereda Id, CreatedAt y UpdatedAt de Entity.
 * Se conecta: ← MailClient / Notificadores · → MessagesQuery, pestaña Mensajes
 * Reglas:     RF-12 · RF-15 · RF-35
 * Estado:     Completo
 */

using Acompana.Domain.Common;
using Acompana.Domain.Enums;

namespace Acompana.Domain.Oversight;

public class OutboundMessage : Entity
{
    public MessageChannel Channel { get; set; } = MessageChannel.Email;
    public string ToAddress { get; set; } = "";
    public MessageKind Kind { get; set; }
    public string Subject { get; set; } = "";
    public string Body { get; set; } = "";
    public Guid? ServiceId { get; set; }
}
