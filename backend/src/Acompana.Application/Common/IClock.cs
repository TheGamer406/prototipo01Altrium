/*
 * Archivo:    backend/src/Acompana.Application/Common/IClock.cs
 * Qué hace:   Hora actual inyectable (pruebas deterministas).
 * Recibe:     —
 * Procesa:    Now (DateTimeOffset UTC) y Today en America/Costa_Rica.
 * Devuelve:   —
 * Se conecta: ← handlers, jobs, seed · → Infrastructure/Time/CostaRicaClock
 * Estado:     Completo
 */

namespace Acompana.Application.Common;

public interface IClock
{
    DateTimeOffset Now { get; }
    DateOnly Today { get; }
}
