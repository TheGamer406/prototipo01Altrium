/*
 * Archivo:    backend/src/Acompana.Application/Admin/IDemoSeeder.cs
 * Qué hace:   Contrato del sembrador de datos del demo.
 * Recibe:     —
 * Procesa:    Interfaz; implementada en Infrastructure/Seed/DemoSeeder.
 * Devuelve:   —
 * Se conecta: ← ResetDemoHandler, DatabaseInitializer
 * Estado:     Completo
 *
 * Contrato:
 *   Task<SeedSummary> SeedAsync(int seed, CancellationToken ct)
 *     Recibe:     semilla (2026)
 *     Procesa:    ver DemoSeeder
 *     Devuelve:   conteos sembrados
 */

namespace Acompana.Application.Admin;

public record SeedSummary(int Branches, int Employees, int Services, int Providers, int Components, int Packages);

public interface IDemoSeeder
{
    Task<SeedSummary> SeedAsync(int seed, CancellationToken ct);
}
