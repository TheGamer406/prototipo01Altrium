/*
 * Archivo:    backend/src/Acompana.Infrastructure/Seed/PeopleSeed.cs
 * Qué hace:   Personal, proveedores y aseguradoras.
 * Recibe:     Faker con semilla DEMO_SEED (2026) y 'hoy' = fecha del servidor en Costa Rica.
 * Procesa:    ~125 empleados (GAM 2-3 + encargado; fuera 1 que hace todo; 6 operadores; 5 gerencia);
 *               30 proveedores (12 flores, 10 catering, 8 música) por zona; 3 aseguradoras.
 * Devuelve:   Listas
 * Se conecta: ← DemoSeeder / ResetDemoHandler · → AppDbContext
 * Reglas:     Datos ficticios; cédulas formato 1-0234-0567
 * Estado:     Completo
 */

using Acompana.Domain.Catalog;
using Acompana.Domain.Enums;
using Acompana.Domain.Organization;
using Acompana.Domain.Providers;

namespace Acompana.Infrastructure.Seed;

public static class PeopleSeed
{
    public static void Run(SeedContext c)
    {
        var f = c.Faker;
        string Name() => $"{f.Name.FirstName()} {f.Name.LastName()} {f.Name.LastName()}";

        var emps = new List<Employee>();
        var crematoriumBranch = c.Branches.First(b => b.HasCrematorium);
        foreach (var b in c.Branches)
        {
            if (b.Zone == Zone.GAM)
            {
                emps.Add(new Employee { BranchId = b.Id, FullName = Name(), Role = EmployeeRole.BranchManager, Shift = Shift.Day });
                for (var i = 0; i < 3; i++)
                    emps.Add(new Employee { BranchId = b.Id, FullName = Name(), Role = EmployeeRole.Advisor, Shift = i == 2 ? Shift.Night : Shift.Day });
                for (var i = 0; i < 3; i++)
                    emps.Add(new Employee { BranchId = b.Id, FullName = Name(), Role = EmployeeRole.Driver, Shift = i == 2 ? Shift.Night : Shift.Day });
            }
            else
            {
                // Sede remota: una persona hace todo (asesor + encargado) y un chofer
                emps.Add(new Employee { BranchId = b.Id, FullName = Name(), Role = EmployeeRole.Advisor, Shift = Shift.Day });
                emps.Add(new Employee { BranchId = b.Id, FullName = Name(), Role = EmployeeRole.Driver, Shift = Shift.Day });
            }
        }
        for (var i = 0; i < 6; i++)
            emps.Add(new Employee { BranchId = crematoriumBranch.Id, FullName = Name(), Role = EmployeeRole.CrematoriumOperator, Shift = i % 2 == 0 ? Shift.Day : Shift.Night });
        for (var i = 0; i < 5; i++)
            emps.Add(new Employee { BranchId = crematoriumBranch.Id, FullName = Name(), Role = EmployeeRole.Management, Shift = Shift.Day });
        c.Db.Employees.AddRange(emps);
        c.Employees = emps;

        var providers = new List<Provider>();
        void Add(ProviderType t, string[] names, int gam)
        {
            for (var i = 0; i < names.Length; i++)
                providers.Add(new Provider
                {
                    Name = names[i], Type = t, Zone = i < gam ? Zone.GAM : Zone.Fuera, Active = true,
                    Email = $"contacto{providers.Count + 1}@{Slug(names[i])}.example.cr", Phone = c.Phone()
                });
        }
        Add(ProviderType.Flowers, new[]
        {
            "Floristería Las Orquídeas", "Flores del Valle", "Jardín de Rosas", "Floristería Margarita", "Arreglos Primavera",
            "Floristería El Girasol", "Flores La Candelaria", "Casa de las Flores", "Floristería Guanacaste", "Flores del Pacífico",
            "Floristería Caribe", "Flores Zona Sur"
        }, 8);
        Add(ProviderType.Catering, new[]
        {
            "Catering Don Mario", "Sabores del Valle", "Banquetes La Hacienda", "Café y Pan Tradicional", "Catering Sazón Tico",
            "Delicias Doña Rosa", "Banquetes El Roble", "Catering Costa Norte", "Sabores del Pacífico", "Catering Sur Gourmet"
        }, 7);
        Add(ProviderType.Music, new[]
        {
            "Mariachi Los Cipreses del Valle", "Cuarteto de Cuerdas Armonía", "Violines de Heredia", "Mariachi Tierra Ticaba",
            "Dúo Sacro Alajuela", "Mariachi Guanacaste", "Música Sacra del Pacífico", "Trío Costa Sur"
        }, 5);
        c.Db.Providers.AddRange(providers);
        c.Providers = providers;

        var insurers = new[] { "Seguros Mutual del Istmo", "Aseguradora Pura Vida", "Protección Familiar Centroamericana" }
            .Select((n, i) => new Insurer { Name = n, NoticeEmail = $"avisos{i + 1}@{Slug(n)}.example.cr" }).ToList();
        c.Db.Insurers.AddRange(insurers);
        c.Insurers = insurers;
        c.Db.SaveChanges();
    }

    private static string Slug(string s)
    {
        var norm = s.ToLowerInvariant().Normalize(System.Text.NormalizationForm.FormD);
        var chars = norm.Where(ch => char.IsLetterOrDigit(ch) && ch < 128).ToArray();
        return new string(chars);
    }
}
