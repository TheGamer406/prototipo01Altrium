/*
 * Archivo:    backend/src/Acompana.Infrastructure/Seed/SeedContext.cs
 * Qué hace:   Estado compartido por los pasos del seed (azar con semilla fija, reloj, listas ya guardadas).
 * Recibe:     AppDbContext, semilla (DEMO_SEED), 'ahora' (UTC) y 'hoy' en Costa Rica.
 * Procesa:    Expone Random y Faker deterministas, cédulas ficticias (1-0234-0567, nunca terminan en 9
 *               salvo que se pida), consecutivos SRV-AAAA-00000 y búsquedas en las listas sembradas.
 * Devuelve:   Helpers para BranchSeed, CatalogSeed, PeopleSeed, HistorySeed y TodaySeed.
 * Se conecta: ← DemoSeeder · → todos los pasos del seed
 * Reglas:     Datos ficticios; cédulas formato 1-0234-0567
 * Estado:     Completo
 */
using Acompana.Domain.Catalog;
using Acompana.Domain.Enums;
using Acompana.Domain.Organization;
using Acompana.Domain.Providers;
using Acompana.Infrastructure.Persistence;
using Bogus;

namespace Acompana.Infrastructure.Seed;

public sealed class SeedContext
{
    public SeedContext(AppDbContext db, int seed, DateTimeOffset now, DateOnly today, IReadOnlyDictionary<string, string> settings)
    {
        Db = db;
        Rng = new Random(seed);
        Faker = new Faker("es") { Random = new Randomizer(seed) };
        Now = now;
        Today = today;
        Settings = settings;
    }

    public AppDbContext Db { get; }
    public Random Rng { get; }
    public Faker Faker { get; }
    public DateTimeOffset Now { get; }
    public DateOnly Today { get; }
    public IReadOnlyDictionary<string, string> Settings { get; }

    public List<Branch> Branches { get; set; } = new();
    public List<Employee> Employees { get; set; } = new();
    public List<Provider> Providers { get; set; } = new();
    public List<Insurer> Insurers { get; set; } = new();
    public List<Component> Components { get; set; } = new();
    public List<Package> Packages { get; set; } = new();
    public List<CasketModel> Caskets { get; set; } = new();

    public int InvoiceSeq { get; set; }
    public int CoreSeq { get; set; } = 1000;

    private readonly Dictionary<int, int> _codes = new();

    public string NextCode(int year)
    {
        _codes[year] = _codes.GetValueOrDefault(year) + 1;
        return $"SRV-{year}-{_codes[year]:D5}";
    }

    public T Pick<T>(IReadOnlyList<T> list) => list[Rng.Next(list.Count)];

    public string NationalId(bool endsInNine = false)
    {
        var last = endsInNine ? 9 : Rng.Next(0, 9);
        var tail = Rng.Next(0, 1000) * 10 + last;
        return $"{Rng.Next(1, 8)}-{Rng.Next(100, 1000):D4}-{tail:D4}";
    }

    public string Phone() => $"{(Rng.Next(0, 2) == 0 ? "8" : "6")}{Rng.Next(100, 999)}-{Rng.Next(1000, 9999)}";

    public Branch BranchByName(string name) => Branches.First(b => b.Name == name);

    public Package PackageByName(string name) => Packages.First(p => p.Name == name);
}
