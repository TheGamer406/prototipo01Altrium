/*
 * Archivo:    backend/src/Acompana.Infrastructure/Seed/CatalogSeed.cs
 * Qué hace:   Componentes, paquetes, modelos de cofre, stock y parámetros.
 * Recibe:     Faker con semilla DEMO_SEED (2026) y 'hoy' = fecha del servidor en Costa Rica.
 * Procesa:    ~25 componentes (algunos 50 % o 100 % exentos); 4 paquetes (Básico ₡850 000, Tradicional
 *               ₡1 450 000, Premium ₡2 600 000, Cremación ₡1 200 000 = suma de componentes); 10 cofres
 *               (6 nacionales, 4 de España, ₡350 000-₡1 900 000, imagen cofreN.webp); stock con 3-4
 *               sedes en mínimo o cero.
 * Devuelve:   Listas de entidades
 * Se conecta: ← DemoSeeder / ResetDemoHandler · → AppDbContext
 * Reglas:     Datos ficticios; cédulas formato 1-0234-0567
 * Estado:     Completo
 */

using Acompana.Domain.Catalog;
using Acompana.Domain.Enums;
using Acompana.Domain.Inventory;
using Acompana.Domain.Oversight;

namespace Acompana.Infrastructure.Seed;

public static class CatalogSeed
{
    public static void Run(SeedContext c)
    {
        Component K(string name, ComponentCategory cat, decimal price, int exempt = 0, ProviderType? provider = null, string? img = null) =>
            new() { Name = name, Category = cat, PriceCrc = price, TaxExemptPct = exempt, RequiresProvider = provider != null, ProviderType = provider, ImagePath = img };

        var comps = new List<Component>
        {
            K("Cofre línea económica", ComponentCategory.Casket, 350_000),
            K("Cofre línea clásica", ComponentCategory.Casket, 600_000),
            K("Cofre línea premium", ComponentCategory.Casket, 1_100_000),
            K("Preparación básica del cuerpo", ComponentCategory.BodyPreparation, 80_000),
            K("Embalsamado y maquillaje", ComponentCategory.BodyPreparation, 150_000),
            K("Carroza fúnebre", ComponentCategory.Hearse, 120_000),
            K("Sala de velación 24 h", ComponentCategory.Chapel, 140_000),
            K("Misa en iglesia", ComponentCategory.Church, 60_000, 100),
            K("Trámites y permisos", ComponentCategory.Extra, 50_000),
            K("Arreglo floral sencillo", ComponentCategory.Flowers, 70_000, 0, ProviderType.Flowers, "img/florales/flores1.webp"),
            K("Corona de flores", ComponentCategory.Flowers, 120_000, 0, ProviderType.Flowers, "img/florales/flores2.webp"),
            K("Arreglo floral premium", ComponentCategory.Flowers, 220_000, 0, ProviderType.Flowers, "img/florales/flores3.webp"),
            K("Centro de mesa floral", ComponentCategory.Flowers, 65_000, 0, ProviderType.Flowers, "img/florales/flores4.webp"),
            K("Ramo de condolencia", ComponentCategory.Flowers, 40_000, 0, ProviderType.Flowers, "img/florales/flores5.webp"),
            K("Cruz floral", ComponentCategory.Flowers, 150_000, 0, ProviderType.Flowers, "img/florales/flores6.webp"),
            K("Café y bocadillos", ComponentCategory.Catering, 90_000, 0, ProviderType.Catering, "img/otros/catering1.webp"),
            K("Catering completo", ComponentCategory.Catering, 220_000, 0, ProviderType.Catering, "img/otros/catering2.webp"),
            K("Desayuno para la familia", ComponentCategory.Catering, 110_000, 0, ProviderType.Catering, "img/otros/catering3.webp"),
            K("Música en vivo (violín)", ComponentCategory.Music, 110_000, 0, ProviderType.Music),
            K("Mariachi", ComponentCategory.Music, 260_000, 0, ProviderType.Music, "img/otros/mariachi1.webp"),
            K("Trío de cuerdas", ComponentCategory.Music, 180_000, 0, ProviderType.Music, "img/otros/mariachi2.webp"),
            K("Cremación", ComponentCategory.Cremation, 380_000, 50),
            K("Urna cineraria", ComponentCategory.Extra, 80_000),
            K("Derecho de sepultura", ComponentCategory.Extra, 200_000, 50),
            K("Transmisión en línea", ComponentCategory.Extra, 50_000),
            K("Libro de condolencias", ComponentCategory.Extra, 10_000, 100),
            K("Tarjetas de recordatorio", ComponentCategory.Extra, 30_000),
        };
        c.Db.Components.AddRange(comps);
        c.Db.SaveChanges();
        c.Components = comps;
        Component C(string n) => comps.First(x => x.Name == n);

        Package P(string name, string desc, string img, params (string Comp, int Qty)[] items) => new()
        {
            Name = name, Description = desc, ImagePath = img,
            Items = items.Select(i => new PackageItem { ComponentId = C(i.Comp).Id, Component = C(i.Comp), Qty = i.Qty }).ToList()
        };

        var packages = new List<Package>
        {
            P("Básico", "Lo esencial para despedir a un ser querido con dignidad.", "img/otros/velorioMuestra1.webp",
                ("Cofre línea económica", 1), ("Preparación básica del cuerpo", 1), ("Carroza fúnebre", 1), ("Sala de velación 24 h", 1),
                ("Trámites y permisos", 1), ("Arreglo floral sencillo", 1), ("Libro de condolencias", 1), ("Tarjetas de recordatorio", 1)),
            P("Tradicional", "Velación completa con misa, flores, café y música.", "img/otros/velorioMuestra2.webp",
                ("Cofre línea clásica", 1), ("Embalsamado y maquillaje", 1), ("Carroza fúnebre", 1), ("Sala de velación 24 h", 1),
                ("Misa en iglesia", 1), ("Trámites y permisos", 1), ("Corona de flores", 1), ("Café y bocadillos", 1),
                ("Música en vivo (violín)", 1), ("Libro de condolencias", 1)),
            P("Premium", "Atención integral con catering, mariachi y transmisión en línea.", "img/otros/velorioMuestra3.webp",
                ("Cofre línea premium", 1), ("Embalsamado y maquillaje", 1), ("Carroza fúnebre", 1), ("Sala de velación 24 h", 1),
                ("Misa en iglesia", 1), ("Trámites y permisos", 1), ("Arreglo floral premium", 1), ("Corona de flores", 1),
                ("Arreglo floral sencillo", 1), ("Catering completo", 1), ("Mariachi", 1), ("Transmisión en línea", 1),
                ("Libro de condolencias", 1), ("Tarjetas de recordatorio", 1)),
            P("Cremación", "Cremación con velación breve y urna incluida.", "img/otros/velorioMuestra1.webp",
                ("Cofre línea económica", 1), ("Preparación básica del cuerpo", 1), ("Carroza fúnebre", 1), ("Sala de velación 24 h", 1),
                ("Trámites y permisos", 1), ("Cremación", 1), ("Urna cineraria", 1)),
        };
        c.Db.Packages.AddRange(packages);
        c.Db.SaveChanges();
        c.Packages = packages;

        // 10 modelos de cofre: 6 nacionales y 4 importados de España
        var caskets = new List<CasketModel>
        {
            new() { Sku = "COF-001", Name = "Pino Sencillo", Material = "Pino", Origin = CasketOrigin.Nacional, PriceCrc = 350_000 },
            new() { Sku = "COF-002", Name = "Cedro Clásico", Material = "Cedro", Origin = CasketOrigin.Nacional, PriceCrc = 520_000 },
            new() { Sku = "COF-003", Name = "Roble Heredia", Material = "Roble", Origin = CasketOrigin.Nacional, PriceCrc = 680_000 },
            new() { Sku = "COF-004", Name = "Caoba Tradicional", Material = "Caoba", Origin = CasketOrigin.Nacional, PriceCrc = 850_000 },
            new() { Sku = "COF-005", Name = "Laurel Pulido", Material = "Laurel", Origin = CasketOrigin.Nacional, PriceCrc = 990_000 },
            new() { Sku = "COF-006", Name = "Cenízaro Real", Material = "Cenízaro", Origin = CasketOrigin.Nacional, PriceCrc = 1_150_000 },
            new() { Sku = "COF-007", Name = "Sevilla", Material = "Nogal", Origin = CasketOrigin.Importado, PriceCrc = 1_300_000 },
            new() { Sku = "COF-008", Name = "Toledo", Material = "Roble macizo", Origin = CasketOrigin.Importado, PriceCrc = 1_500_000 },
            new() { Sku = "COF-009", Name = "Granada", Material = "Caoba y bronce", Origin = CasketOrigin.Importado, PriceCrc = 1_700_000 },
            new() { Sku = "COF-010", Name = "Madrid Imperial", Material = "Caoba y herrajes dorados", Origin = CasketOrigin.Importado, PriceCrc = 1_900_000 },
        };
        for (var i = 0; i < caskets.Count; i++) caskets[i].ImagePath = $"img/cofres/cofre{i + 1}.webp";
        c.Db.CasketModels.AddRange(caskets);
        c.Db.SaveChanges();
        c.Caskets = caskets;

        var defaultMin = int.Parse(c.Settings.GetValueOrDefault("DefaultMinStock", "2"));
        var stock = new List<CasketStock>();
        foreach (var b in c.Branches)
            foreach (var m in caskets)
                stock.Add(new CasketStock { BranchId = b.Id, CasketModelId = m.Id, Qty = 3 + c.Rng.Next(0, 6), MinQty = defaultMin });

        // 3-4 sedes en mínimo o en cero (RF-27); Heredia queda a una venta del mínimo para el guion
        void Set(string branch, string sku, int qty)
        {
            var b = c.BranchByName(branch); var m = caskets.First(x => x.Sku == sku);
            stock.First(s => s.BranchId == b.Id && s.CasketModelId == m.Id).Qty = qty;
        }
        Set("Heredia", "COF-002", defaultMin + 1);
        Set("Heredia", "COF-003", defaultMin + 1);
        Set("San José Centro", "COF-005", 0);
        Set("Cartago", "COF-001", defaultMin);
        Set("Liberia", "COF-007", defaultMin - 1);
        c.Db.CasketStock.AddRange(stock);

        c.Db.Settings.AddRange(
            new Setting { Key = Setting.ProviderAcceptTimeoutMin, Value = c.Settings.GetValueOrDefault("ProviderAcceptTimeoutMin", "30") },
            new Setting { Key = Setting.TransferThresholdMin, Value = c.Settings.GetValueOrDefault("TransferThresholdMin", "20") },
            new Setting { Key = Setting.DefaultMinStock, Value = defaultMin.ToString() });
        c.Db.SaveChanges();
    }
}
