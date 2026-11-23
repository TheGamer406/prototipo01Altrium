/*
 * Archivo:    backend/src/Acompana.Integrations.Mock/Latency.cs
 * Qué hace:   Simula la latencia de los sistemas externos (300-1200 ms).
 * Recibe:     —
 * Procesa:    Espera un tiempo aleatorio.
 * Devuelve:   Task.
 * Se conecta: ← endpoints del mock
 * Estado:     Completo
 */
namespace Acompana.Integrations.Mock;

public static class Latency
{
    public static Task Wait() => Task.Delay(Random.Shared.Next(300, 1200));
}
