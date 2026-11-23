/*
 * Archivo:    backend/src/Acompana.Application/Integrations/IInsurerClient.cs
 * Qué hace:   Cliente del aviso estructurado a la aseguradora.
 * Recibe:     —
 * Procesa:    Interfaz; la implementación vive en Infrastructure/Integrations.
 * Devuelve:   —
 * Se conecta: ← handlers · → Infrastructure/Integrations (HttpClient al servicio simulado)
 * Reglas:     RF-12
 * Estado:     Completo
 *
 * Contrato:
 *   Task SendNoticeAsync(InsurerNotice notice, CancellationToken ct)
 *     Recibe:     aseguradora, póliza, asegurado (fallecido), código de servicio, monto
 *     Procesa:    POST /aseguradoras/notices
 *     Devuelve:   Task (202)
 */

namespace Acompana.Application.Integrations;

public record InsurerNotice(string Insurer, string Policy, string Insured, string ServiceCode, decimal AmountCrc);

public interface IInsurerClient
{
    Task SendNoticeAsync(InsurerNotice notice, CancellationToken ct);
}
