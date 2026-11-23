/*
 * Archivo:    backend/src/Acompana.Application/Integrations/IMockControlClient.cs
 * Qué hace:   Control del servicio simulado para el modo admin.
 * Recibe:     —
 * Procesa:    Interfaz; la implementación vive en Infrastructure/Integrations.
 * Devuelve:   —
 * Se conecta: ← handlers · → Infrastructure/Integrations (HttpClient al servicio simulado)
 * Reglas:     —
 * Estado:     Completo
 *
 * Contrato:
 *   Task SetCoreDownAsync(int minutes, CancellationToken ct) · Task
 *       RejectNextInvoiceAsync(CancellationToken ct)
 *     Recibe:     minutos de caída / nada
 *     Procesa:    POST /_control/core-down · POST /_control/reject-next
 *     Devuelve:   Task
 */

namespace Acompana.Application.Integrations;

public interface IMockControlClient
{
    Task SetCoreDownAsync(int minutes, CancellationToken ct);
    Task RejectNextInvoiceAsync(CancellationToken ct);
    Task ResetAsync(CancellationToken ct);
}
