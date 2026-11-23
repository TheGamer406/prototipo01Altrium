/*
 * Archivo:    backend/src/Acompana.Infrastructure/Persistence/DesignTimeDbContextFactory.cs
 * Qué hace:   Fábrica de AppDbContext para `dotnet ef` (migraciones y script de esquema).
 * Recibe:     Variable de entorno ConnectionStrings__Default (opcional).
 * Procesa:    Crea las opciones Npgsql con nombres snake_case.
 * Devuelve:   AppDbContext solo para herramientas de diseño.
 * Se conecta: ← dotnet ef · → AppDbContext
 * Estado:     Completo
 */
using Acompana.Application.Common;
using Acompana.Domain.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace Acompana.Infrastructure.Persistence;

public class DesignTimeDbContextFactory : IDesignTimeDbContextFactory<AppDbContext>
{
    public AppDbContext CreateDbContext(string[] args)
    {
        var cs = Environment.GetEnvironmentVariable("ConnectionStrings__Default")
                 ?? "Host=localhost;Port=5432;Database=acompana;Username=acompana;Password=design";
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseNpgsql(cs)
            .UseSnakeCaseNamingConvention()
            .Options;
        return new AppDbContext(options);
    }
}
