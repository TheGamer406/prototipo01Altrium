/*
 * Archivo:    backend/src/Acompana.Infrastructure/Seed/HistorySeed.cs
 * Qué hace:   12 meses de servicios históricos coherentes para la analítica.
 * Recibe:     Faker con semilla DEMO_SEED (2026) y 'hoy' = fecha del servidor en Costa Rica.
 * Procesa:    ~1 800 servicios, diciembre y enero al doble; ~30 % personalizados, ~35 % cremación, ~25
 *               % aseguradora; historial de estados con tiempos realistas; facturas y pagos.
 * Devuelve:   Servicios
 * Se conecta: ← DemoSeeder / ResetDemoHandler · → AppDbContext
 * Reglas:     Datos ficticios; cédulas formato 1-0234-0567
 * Estado:     Completo
 */

using Acompana.Domain.Enums;
using Acompana.Domain.Organization;

namespace Acompana.Infrastructure.Seed;

public static class HistorySeed
{
    private const int Total = 1800;
    private static readonly string[] SlowBranches = { "Quepos", "Golfito", "Ciudad Neily" };

    public static int Run(SeedContext c)
    {
        var first = new DateOnly(c.Today.Year, c.Today.Month, 1).AddMonths(-11);
        var months = Enumerable.Range(0, 12).Select(i => first.AddMonths(i)).ToList();
        double W(DateOnly m) => m.Month is 12 or 1 ? 2.0 : 1.0;
        var sumW = months.Sum(W);

        var weighted = c.Branches.SelectMany(b => Enumerable.Repeat(b, b.Zone == Zone.GAM ? 3 : 1)).ToList();
        var packWeights = new[] { ("Básico", 25), ("Tradicional", 35), ("Premium", 12), ("Cremación", 28) };
        var packBag = packWeights.SelectMany(p => Enumerable.Repeat(p.Item1, p.Item2)).ToList();
        var maxDay = c.Today.AddDays(-3);
        var created = 0;

        foreach (var m in months)
        {
            var count = (int)Math.Round(Total * W(m) / sumW);
            var maxMonth = new DateOnly(maxDay.Year, maxDay.Month, 1);
            var lastDay = m > maxMonth ? 0 : m == maxMonth ? maxDay.Day : DateTime.DaysInMonth(m.Year, m.Month);
            if (lastDay < 1) continue;

            var batch = new List<Domain.Services.Service>();
            for (var i = 0; i < count; i++)
            {
                var branch = c.Pick(weighted);
                var pkg = c.PackageByName(c.Pick(packBag));
                var localHour = c.Rng.Next(8, 17);
                var day = new DateTime(m.Year, m.Month, c.Rng.Next(1, lastDay + 1), localHour, c.Rng.Next(0, 4) * 15, 0, DateTimeKind.Utc);
                var scheduled = new DateTimeOffset(day.AddHours(6), TimeSpan.Zero); // hora de Costa Rica -> UTC
                var createdAt = scheduled.AddHours(-c.Rng.Next(6, 60));
                var insurer = c.Rng.Next(100) < 25;
                var custom = c.Rng.Next(100) < 30;
                var cremation = pkg.Name == "Cremación" || c.Rng.Next(100) < 7;

                var s = ServiceBuilder.Build(c, branch, pkg, scheduled, createdAt, ServiceKind.Immediate, ServiceStatus.Completed, custom, insurer, cremation);
                s.Branch = null;

                var slow = SlowBranches.Contains(branch.Name) ? 3.0 : branch.Zone == Zone.Fuera ? 1.8 : 1.0;
                var t1 = createdAt.AddMinutes(c.Rng.Next(40, 240));
                var t2 = t1.AddMinutes(c.Rng.Next(90, 600) * slow);
                var t3 = t2.AddMinutes(60) > scheduled ? t2.AddMinutes(60) : scheduled;
                var t4 = t3.AddHours(c.Rng.Next(24, 31));
                var actor = ServiceBuilder.Advisor(branch);
                s.History.Add(new() { ServiceId = s.Id, FromStatus = ServiceStatus.Registered, ToStatus = ServiceStatus.Invoicing, ChangedBy = actor, ChangedAt = t1, CreatedAt = t1 });
                s.History.Add(new() { ServiceId = s.Id, FromStatus = ServiceStatus.Invoicing, ToStatus = ServiceStatus.Paid, ChangedBy = actor, ChangedAt = t2, CreatedAt = t2 });
                s.History.Add(new() { ServiceId = s.Id, FromStatus = ServiceStatus.Paid, ToStatus = ServiceStatus.InProgress, ChangedBy = actor, ChangedAt = t3, CreatedAt = t3 });
                s.History.Add(new() { ServiceId = s.Id, FromStatus = ServiceStatus.InProgress, ToStatus = ServiceStatus.Completed, ChangedBy = actor, ChangedAt = t4, CreatedAt = t4 });

                var inv = ServiceBuilder.AddInvoice(c, s, InvoiceStatus.Accepted, t1);
                ServiceBuilder.AddPayment(c, inv, s, t2);
                foreach (var l in s.Lines)
                {
                    l.Status = LineStatus.Delivered;
                    if (l.Component?.RequiresProvider == true)
                    {
                        var pool = c.Providers.Where(p => p.Type == l.Component.ProviderType && p.Zone == branch.Zone).ToList();
                        l.ProviderId = c.Pick(pool).Id;
                        l.AcceptedPriceCrc = l.UnitPriceCrc;
                        l.AcceptedAt = t2.AddMinutes(c.Rng.Next(5, 25));
                    }
                    l.Component = null;
                }
                s.UpdatedAt = t4;
                batch.Add(s);
                created++;
            }
            c.Db.Services.AddRange(batch);
            c.Db.SaveChanges();
            c.Db.ChangeTracker.Clear();
        }
        return created;
    }
}
