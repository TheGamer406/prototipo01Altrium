/*
 * Archivo:    backend/src/Acompana.Infrastructure/Time/CostaRicaClock.cs
 * Qué hace:   Implementa IClock con la zona America/Costa_Rica.
 * Recibe:     —
 * Procesa:    Now = DateTimeOffset.UtcNow; Today = fecha local de Costa Rica.
 * Devuelve:   —
 * Se conecta: ← DependencyInjection
 * Estado:     Completo
 */

using Acompana.Application.Common;

namespace Acompana.Infrastructure.Time;

public sealed class CostaRicaClock : IClock
{
    private static readonly TimeZoneInfo Tz = Resolve();

    private static TimeZoneInfo Resolve()
    {
        try { return TimeZoneInfo.FindSystemTimeZoneById("America/Costa_Rica"); }
        catch { return TimeZoneInfo.CreateCustomTimeZone("CR", TimeSpan.FromHours(-6), "CR", "CR"); }
    }

    public DateTimeOffset Now => DateTimeOffset.UtcNow;

    public DateOnly Today => DateOnly.FromDateTime(TimeZoneInfo.ConvertTime(DateTimeOffset.UtcNow, Tz).DateTime);
}
