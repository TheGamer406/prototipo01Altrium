/*
 * Archivo:    backend/src/Acompana.Infrastructure/Seed/BranchSeed.cs
 * Qué hace:   Sedes, capillas, carrozas y crematorio.
 * Recibe:     Faker con semilla DEMO_SEED (2026) y 'hoy' = fecha del servidor en Costa Rica.
 * Procesa:    28 sedes (12 GAM, 16 fuera) con coordenadas aproximadas del cantón; 2-3 capillas GAM,
 *               0-1 fuera; ~35 carrozas; crematorio en San José.
 * Devuelve:   Listas de entidades
 * Se conecta: ← DemoSeeder / ResetDemoHandler · → AppDbContext
 * Reglas:     Datos ficticios; cédulas formato 1-0234-0567
 * Estado:     Completo
 */

using Acompana.Domain.Enums;
using Acompana.Domain.Logistics;
using Acompana.Domain.Organization;

namespace Acompana.Infrastructure.Seed;

public static class BranchSeed
{
    // (nombre, cantón, provincia, zona, lat, lng)
    private static readonly (string Name, string Canton, string Province, Zone Zone, double Lat, double Lng)[] Data =
    {
        ("San José Centro", "San José", "San José", Zone.GAM, 9.9341, -84.0877),
        ("Escazú", "Escazú", "San José", Zone.GAM, 9.9189, -84.1395),
        ("Santa Ana", "Santa Ana", "San José", Zone.GAM, 9.9328, -84.1833),
        ("Desamparados", "Desamparados", "San José", Zone.GAM, 9.8975, -84.0658),
        ("Goicoechea", "Goicoechea", "San José", Zone.GAM, 9.9561, -84.0506),
        ("Tibás", "Tibás", "San José", Zone.GAM, 9.9569, -84.0794),
        ("Curridabat", "Curridabat", "San José", Zone.GAM, 9.9142, -84.0344),
        ("Heredia", "Heredia", "Heredia", Zone.GAM, 9.9981, -84.1198),
        ("Belén", "Belén", "Heredia", Zone.GAM, 9.9778, -84.1853),
        ("Alajuela", "Alajuela", "Alajuela", Zone.GAM, 10.0162, -84.2142),
        ("Cartago", "Cartago", "Cartago", Zone.GAM, 9.8644, -83.9194),
        ("Alajuelita", "Alajuelita", "San José", Zone.GAM, 9.9014, -84.1031),
        ("Liberia", "Liberia", "Guanacaste", Zone.Fuera, 10.6350, -85.4377),
        ("Santa Cruz", "Santa Cruz", "Guanacaste", Zone.Fuera, 10.2606, -85.5848),
        ("Nicoya", "Nicoya", "Guanacaste", Zone.Fuera, 10.1483, -85.4520),
        ("Cañas", "Cañas", "Guanacaste", Zone.Fuera, 10.4263, -85.0975),
        ("Puntarenas Centro", "Puntarenas", "Puntarenas", Zone.Fuera, 9.9763, -84.8384),
        ("Quepos", "Aguirre", "Puntarenas", Zone.Fuera, 9.4316, -84.1620),
        ("Golfito", "Golfito", "Puntarenas", Zone.Fuera, 8.6392, -83.1650),
        ("Ciudad Neily", "Corredores", "Puntarenas", Zone.Fuera, 8.6560, -82.9400),
        ("San Isidro de El General", "Pérez Zeledón", "San José", Zone.Fuera, 9.3739, -83.7017),
        ("Ciudad Quesada", "San Carlos", "Alajuela", Zone.Fuera, 10.3237, -84.4283),
        ("Upala", "Upala", "Alajuela", Zone.Fuera, 10.8987, -85.0165),
        ("Guápiles", "Pococí", "Limón", Zone.Fuera, 10.2145, -83.7870),
        ("Siquirres", "Siquirres", "Limón", Zone.Fuera, 10.0990, -83.5070),
        ("Limón Centro", "Limón", "Limón", Zone.Fuera, 9.9907, -83.0360),
        ("Turrialba", "Turrialba", "Cartago", Zone.Fuera, 9.9040, -83.6840),
        ("Puerto Viejo de Sarapiquí", "Sarapiquí", "Heredia", Zone.Fuera, 10.4560, -84.0100),
    };

    public static void Run(SeedContext c)
    {
        var branches = Data.Select((d, i) => new Branch
        {
            Code = $"SED-{i + 1:D2}",
            Name = d.Name, Canton = d.Canton, Province = d.Province, Zone = d.Zone, Lat = d.Lat, Lng = d.Lng,
            HasChapels = d.Zone == Zone.GAM || i % 3 == 0,
            HasCrematorium = d.Name == "San José Centro",
        }).ToList();
        c.Db.Branches.AddRange(branches);
        c.Db.SaveChanges();
        c.Branches = branches;

        var chapels = new List<Chapel>();
        foreach (var b in branches.Where(b => b.HasChapels))
        {
            var n = b.Zone == Zone.GAM ? (b.Name == "Heredia" || b.Id % 3 == 0 ? 3 : 2) : 1;
            for (var i = 1; i <= n; i++)
                chapels.Add(new Chapel { BranchId = b.Id, Name = $"Capilla {(char)('A' + i - 1)}", Capacity = 40 + c.Rng.Next(0, 5) * 10 });
        }
        c.Db.Chapels.AddRange(chapels);

        var hearses = new List<Hearse>();
        var plateN = 100;
        foreach (var b in branches)
        {
            var n = b.Zone == Zone.GAM ? 2 : 1;
            if (b.Name == "San José Centro" || b.Name == "Alajuela" || b.Name == "Heredia") n = 3;
            for (var i = 0; i < n && hearses.Count < 35; i++)
                hearses.Add(new Hearse { BranchId = b.Id, Plate = $"FUN-{plateN++}", Status = HearseStatus.Available });
        }
        c.Db.Hearses.AddRange(hearses);
        c.Db.SaveChanges();
    }
}
