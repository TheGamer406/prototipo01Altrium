/*
 * Archivo:    backend/src/Acompana.Integrations.Mock/MockState.cs
 * Qué hace:   Estado en memoria del simulador.
 * Recibe:     —
 * Procesa:    Diccionario Idempotency-Key → respuesta; CoreDownUntil; RejectNext (bool); consecutivos
 *               FE-/BOD-.
 * Devuelve:   —
 * Se conecta: ← todos los endpoints del mock
 * Reglas:     RF-13
 * Estado:     Completo
 */

using System.Collections.Concurrent;

namespace Acompana.Integrations.Mock;

public sealed class MockState
{
    public ConcurrentDictionary<string, object> Responses { get; } = new();
    public DateTimeOffset CoreDownUntil { get; set; } = DateTimeOffset.MinValue;
    public bool RejectNext { get; set; }
    public ConcurrentDictionary<string, int> Balances { get; } = new();
    private int _fe = 1234, _bod = 0;
    public string NextCoreReference() => $"FE-{Interlocked.Increment(ref _fe):D10}";
    public string NextRestockNumber() => $"BOD-{Interlocked.Increment(ref _bod) + 44:D6}";
}
