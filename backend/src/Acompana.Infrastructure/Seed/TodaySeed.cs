/*
 * Archivo:    backend/src/Acompana.Infrastructure/Seed/TodaySeed.cs
 * Qué hace:   Servicios de hoy y los casos del guion.
 * Recibe:     Faker con semilla DEMO_SEED (2026) y 'hoy' = fecha del servidor en Costa Rica.
 * Procesa:    15-20 servicios: 2 anticipados, 3 sin acta, 2 en facturación (1 rechazado), 4 pagados
 *               con ofertas abiertas, 6 en curso, 2 finalizados; 1 línea urgente (< 2 h); 1 oferta
 *               vencida con alerta; caso de las flores en audit_log; correos simulados; traslados (1 sin
 *               señal sincronizado, 1 con demora); 6 cuerpos en la cola del crematorio.
 * Devuelve:   Servicios del día
 * Se conecta: ← DemoSeeder / ResetDemoHandler · → AppDbContext
 * Reglas:     Datos ficticios; cédulas formato 1-0234-0567
 * Estado:     Completo
 */

using System.Text.Json;
using Acompana.Domain.Billing;
using Acompana.Domain.Enums;
using Acompana.Domain.Logistics;
using Acompana.Domain.Organization;
using Acompana.Domain.Oversight;
using Acompana.Domain.Providers;
using Acompana.Domain.Services;
using Microsoft.EntityFrameworkCore;

namespace Acompana.Infrastructure.Seed;

public static class TodaySeed
{
    public static int Run(SeedContext c)
    {
        var db = c.Db;
        var now = c.Now;
        DateTimeOffset H(double h) => now.AddHours(h);

        var chapels = db.Chapels.AsNoTracking().ToList();
        var hearses = db.Hearses.AsNoTracking().ToList();
        var all = new List<Service>();
        var bookings = new List<ChapelBooking>();
        var offers = new List<ProviderOffer>();
        var alerts = new List<Alert>();
        var messages = new List<OutboundMessage>();
        var transfers = new List<Transfer>();
        var cremations = new List<Cremation>();
        var staff = new List<StaffAssignment>();
        var audit = new List<AuditLog>();
        var usedHearses = new HashSet<long>();
        var comp = c.Components.ToDictionary(k => k.Id);

        Service Make(string branch, string pkg, double scheduledInHours, ServiceStatus status, double createdAgoHours,
            ServiceKind kind = ServiceKind.Immediate, bool custom = false, bool insurer = false, bool? cremation = null, string? familyId = null)
        {
            var b = c.BranchByName(branch);
            var s = ServiceBuilder.Build(c, b, c.PackageByName(pkg), H(scheduledInHours), H(-createdAgoHours), kind, status, custom, insurer, cremation, familyId);
            s.Branch = b;
            if (s.WakePlace == WakePlace.Chapel)
            {
                var free = chapels.Where(x => x.BranchId == b.Id).FirstOrDefault(ch =>
                    !bookings.Any(bk => bk.ChapelId == ch.Id && bk.StartsAt < s.ScheduledAt.AddHours(24) && s.ScheduledAt < bk.EndsAt));
                if (free is null) { s.WakePlace = WakePlace.Church; }
                else bookings.Add(new ChapelBooking { ChapelId = free.Id, ServiceId = s.Id, StartsAt = s.ScheduledAt, EndsAt = s.ScheduledAt.AddHours(24), CreatedAt = s.CreatedAt });
            }
            all.Add(s);
            return s;
        }

        void Certificate(Service s) => s.DeathCertificate = new DeathCertificate
        {
            ServiceId = s.Id, Number = $"ACT-{now.Year}-{c.Rng.Next(10000, 99999)}", FilePath = $"/data/uploads/{s.Id}/acta.pdf",
            RegisteredBy = ServiceBuilder.Advisor(s.Branch!), RegisteredAt = s.CreatedAt.AddMinutes(25), CreatedAt = s.CreatedAt.AddMinutes(25)
        };

        void Hist(Service s, params (ServiceStatus From, ServiceStatus To, double AgoMinutes)[] steps)
        {
            foreach (var (from, to, ago) in steps)
                s.History.Add(new StatusHistory { ServiceId = s.Id, FromStatus = from, ToStatus = to, ChangedBy = ServiceBuilder.Advisor(s.Branch!), ChangedAt = now.AddMinutes(-ago), CreatedAt = now.AddMinutes(-ago) });
        }

        Invoice Invoiced(Service s, double sentAgoMin, bool paid, double paidAgoMin = 0)
        {
            var inv = ServiceBuilder.AddInvoice(c, s, InvoiceStatus.Accepted, now.AddMinutes(-sentAgoMin));
            if (paid) ServiceBuilder.AddPayment(c, inv, s, now.AddMinutes(-paidAgoMin));
            return inv;
        }

        void Msg(Service s, MessageKind kind, string subject, string body, double agoMin, string? to = null) =>
            messages.Add(new OutboundMessage { ToAddress = to ?? s.FamilyContact!.Email, Kind = kind, Subject = subject, Body = body, ServiceId = s.Id, CreatedAt = now.AddMinutes(-agoMin) });

        void Registered(Service s, double agoMin)
        {
            Msg(s, MessageKind.FamilyStatus, $"Servicio {s.Code} registrado",
                $"Estimada familia: su servicio {s.Code} fue registrado en la sede {s.Branch!.Name}. Fecha y hora: {s.ScheduledAt.ToOffset(TimeSpan.FromHours(-6)):dd/MM/yyyy HH:mm}. Le acompañamos en todo momento.", agoMin);
        }

        string Place(Service s) => s.WakePlace == WakePlace.Chapel ? $"Capilla de la sede {s.Branch!.Name}" : s.WakePlace == WakePlace.Church ? "Iglesia indicada por la familia" : s.WakeAddress ?? "Domicilio";

        void OpenOffers(Service s, double sentAgoMin, OfferStatus status)
        {
            foreach (var l in s.Lines.Where(l => comp[l.ComponentId].RequiresProvider))
            {
                var pool = c.Providers.Where(p => p.Active && p.Type == comp[l.ComponentId].ProviderType && p.Zone == s.Branch!.Zone).ToList();
                foreach (var p in pool)
                {
                    offers.Add(new ProviderOffer { ServiceLineId = l.Id, ProviderId = p.Id, SentAt = now.AddMinutes(-sentAgoMin), Status = status,
                        RespondedAt = status == OfferStatus.Expired ? now.AddMinutes(-sentAgoMin + 31) : null, CreatedAt = now.AddMinutes(-sentAgoMin) });
                    messages.Add(new OutboundMessage { ToAddress = p.Email, Kind = MessageKind.ProviderOffer, ServiceId = s.Id, CreatedAt = now.AddMinutes(-sentAgoMin),
                        Subject = $"Nueva oferta: {comp[l.ComponentId].Name}",
                        Body = $"Lugar: {Place(s)}. Fecha: {s.ScheduledAt.ToOffset(TimeSpan.FromHours(-6)):dd/MM/yyyy HH:mm}. Precio fijo: ₡{l.UnitPriceCrc:N0}. Acepte con un clic en la bandeja del proveedor." });
                }
                l.Status = LineStatus.Offered;
            }
        }

        void Award(Service s, double acceptedAgoMin, bool delivered)
        {
            foreach (var l in s.Lines)
            {
                var k = comp[l.ComponentId];
                if (!k.RequiresProvider) { l.Status = delivered ? LineStatus.Delivered : LineStatus.Pending; continue; }
                var pool = c.Providers.Where(p => p.Active && p.Type == k.ProviderType && p.Zone == s.Branch!.Zone).ToList();
                var winner = c.Pick(pool);
                l.ProviderId = winner.Id; l.AcceptedPriceCrc = l.UnitPriceCrc; l.AcceptedAt = now.AddMinutes(-acceptedAgoMin);
                l.Status = delivered ? LineStatus.Delivered : LineStatus.Accepted;
                foreach (var p in pool)
                    offers.Add(new ProviderOffer { ServiceLineId = l.Id, ProviderId = p.Id, SentAt = now.AddMinutes(-acceptedAgoMin - 12),
                        Status = p.Id == winner.Id ? OfferStatus.Won : OfferStatus.Lost, RespondedAt = now.AddMinutes(-acceptedAgoMin), CreatedAt = now.AddMinutes(-acceptedAgoMin - 12) });
            }
        }

        // ---- 2 anticipados
        var adv1 = Make("Cartago", "Premium", 24 * 14, ServiceStatus.Advance, 24 * 9, ServiceKind.Advance);
        var adv2 = Make("Escazú", "Tradicional", 24 * 30, ServiceStatus.Advance, 24 * 4, ServiceKind.Advance);
        foreach (var a in new[] { adv1, adv2 }) { Registered(a, 24 * 60 * 4); }

        // ---- 3 sin acta
        var noCert = new[]
        {
            Make("Heredia", "Tradicional", 20, ServiceStatus.Registered, 2),
            Make("Heredia", "Básico", 30, ServiceStatus.Registered, 1.5),
            Make("Alajuela", "Básico", 26, ServiceStatus.Registered, 3),
        };
        foreach (var s in noCert) Registered(s, 90);

        // ---- 2 en facturación (1 rechazado)
        var inv1 = Make("San José Centro", "Tradicional", 28, ServiceStatus.Invoicing, 4);
        Certificate(inv1); Hist(inv1, (ServiceStatus.Registered, ServiceStatus.Invoicing, 15));
        ServiceBuilder.AddInvoice(c, inv1, InvoiceStatus.Sent, now.AddMinutes(-15));
        var inv2 = Make("Escazú", "Premium", 30, ServiceStatus.Invoicing, 5, insurer: true, familyId: c.NationalId(endsInNine: true));
        Certificate(inv2); Hist(inv2, (ServiceStatus.Registered, ServiceStatus.Invoicing, 40));
        var rejected = ServiceBuilder.AddInvoice(c, inv2, InvoiceStatus.Rejected, now.AddMinutes(-40), "Identificación del receptor inválida", c.NationalId(endsInNine: true));
        foreach (var s in new[] { inv1, inv2 }) Registered(s, 240);
        alerts.Add(new Alert { Type = AlertType.InvoiceRejected, BranchId = inv2.BranchId, ServiceId = inv2.Id, CreatedAt = now.AddMinutes(-39),
            Message = $"El core contable rechazó la factura de {inv2.Code}: {rejected.RejectReason}." });
        Msg(inv1, MessageKind.FamilyInvoice, $"Factura del servicio {inv1.Code}", "Su factura fue enviada al sistema contable y está en proceso de emisión.", 14);

        // ---- 4 pagados con ofertas abiertas
        var paidUrgent = Make("Heredia", "Tradicional", 3, ServiceStatus.Paid, 6);
        var paidOpen = Make("Alajuela", "Premium", 20, ServiceStatus.Paid, 5);
        var paidCrem = Make("Cartago", "Cremación", 30, ServiceStatus.Paid, 4, cremation: true);
        var paidExpired = Make("Belén", "Tradicional", 26, ServiceStatus.Paid, 5, insurer: true);
        foreach (var (s, ago) in new[] { (paidUrgent, 3.0), (paidOpen, 4.0), (paidCrem, 6.0), (paidExpired, 55.0) })
        {
            Certificate(s);
            Hist(s, (ServiceStatus.Registered, ServiceStatus.Invoicing, ago + 40), (ServiceStatus.Invoicing, ServiceStatus.Paid, ago + 1));
            Invoiced(s, ago + 40, true, ago + 1);
            Registered(s, ago + 120);
            if (s.Payer == Payer.Insurer)
                Msg(s, MessageKind.InsurerNotice, $"Aviso de servicio {s.Code}", $"Aviso estructurado a la aseguradora por la póliza {s.PolicyNumber}.", ago + 38, c.Insurers.First(i => i.Id == s.InsurerId).NoticeEmail);
        }
        OpenOffers(paidUrgent, 3, OfferStatus.Open);
        OpenOffers(paidOpen, 4, OfferStatus.Open);
        OpenOffers(paidCrem, 6, OfferStatus.Open);
        OpenOffers(paidExpired, 55, OfferStatus.Expired);
        alerts.Add(new Alert { Type = AlertType.ProviderTimeout, BranchId = paidExpired.BranchId, ServiceId = paidExpired.Id, CreatedAt = now.AddMinutes(-24),
            Message = $"Ningún proveedor aceptó a tiempo los componentes de {paidExpired.Code}. Asigne uno manualmente." });

        // ---- 6 en curso
        var ip1 = Make("Heredia", "Tradicional", -5, ServiceStatus.InProgress, 30, custom: true);
        // caso de las flores: la corona de flores fue eliminada del servicio
        var removedLineId = Guid.NewGuid();
        var removed = ip1.Lines.FirstOrDefault(l => l.ComponentId == c.Components.First(k => k.Name == "Corona de flores").Id);
        if (removed != null) ip1.Lines.Remove(removed);
        ip1.IsCustomPackage = true; ip1.RecalculateTotal();
        var ip2 = Make("San José Centro", "Cremación", -4, ServiceStatus.InProgress, 22, cremation: true);
        var ip3 = Make("Alajuela", "Tradicional", -2, ServiceStatus.InProgress, 20);
        var ip4 = Make("Liberia", "Cremación", -1.5, ServiceStatus.InProgress, 18, cremation: true);
        var ip5 = Make("Desamparados", "Cremación", -10, ServiceStatus.InProgress, 36, cremation: true);
        var ip6 = Make("Escazú", "Cremación", -3, ServiceStatus.InProgress, 21, cremation: true);
        foreach (var s in new[] { ip1, ip2, ip3, ip4, ip5, ip6 })
        {
            var start = (now - s.ScheduledAt).TotalMinutes;
            Certificate(s);
            Hist(s, (ServiceStatus.Registered, ServiceStatus.Invoicing, start + 600), (ServiceStatus.Invoicing, ServiceStatus.Paid, start + 540), (ServiceStatus.Paid, ServiceStatus.InProgress, start));
            Invoiced(s, start + 600, true, start + 540);
            Award(s, start + 480, delivered: false);
            Registered(s, start + 900);
            Msg(s, MessageKind.FamilyStatus, $"Servicio {s.Code} en curso", $"Su servicio {s.Code} ya está en curso en la sede {s.Branch!.Name}.", start);
        }
        // bitácora del caso de las flores (RF-31)
        audit.Add(new AuditLog { At = H(-30), ActorRole = "Advisor", ActorName = "Asesor · Heredia", BranchId = ip1.BranchId, Entity = "Service", EntityId = ip1.Id.ToString(), ServiceId = ip1.Id, Action = AuditAction.Create,
            OldValues = "{}", NewValues = JsonSerializer.Serialize(new { code = ip1.Code, status = "Registered" }), CreatedAt = H(-30) });
        audit.Add(new AuditLog { At = H(-29), ActorRole = "Advisor", ActorName = "Asesor · Heredia", BranchId = ip1.BranchId, Entity = "ServiceLine", EntityId = removedLineId.ToString(), ServiceId = ip1.Id, Action = AuditAction.Delete,
            OldValues = JsonSerializer.Serialize(new { componentName = "Corona de flores", qty = 1, unitPriceCrc = 120000, taxExemptPct = 0, status = "Pending" }), NewValues = "{}", CreatedAt = H(-29) });
        audit.Add(new AuditLog { At = H(-29), ActorRole = "Advisor", ActorName = "Asesor · Heredia", BranchId = ip1.BranchId, Entity = "Service", EntityId = ip1.Id.ToString(), ServiceId = ip1.Id, Action = AuditAction.Update,
            OldValues = JsonSerializer.Serialize(new { isCustomPackage = false, totalCrc = 1450000 }), NewValues = JsonSerializer.Serialize(new { isCustomPackage = true, totalCrc = ip1.TotalCrc }), CreatedAt = H(-29) });

        // ---- 2 finalizados
        var done1 = Make("Curridabat", "Cremación", -40, ServiceStatus.Completed, 70, cremation: true);
        var done2 = Make("Santa Ana", "Tradicional", -38, ServiceStatus.Completed, 66);
        foreach (var s in new[] { done1, done2 })
        {
            var start = (now - s.ScheduledAt).TotalMinutes;
            Certificate(s);
            Hist(s, (ServiceStatus.Registered, ServiceStatus.Invoicing, start + 600), (ServiceStatus.Invoicing, ServiceStatus.Paid, start + 540), (ServiceStatus.Paid, ServiceStatus.InProgress, start), (ServiceStatus.InProgress, ServiceStatus.Completed, 60));
            Invoiced(s, start + 600, true, start + 540);
            Award(s, start + 480, delivered: true);
            Registered(s, start + 900);
        }

        // ---- personal: traslape del asesor de Heredia entre ip1 y el servicio urgente
        long Advisor(string branch, int skip = 0) => c.Employees.Where(e => e.BranchId == c.BranchByName(branch).Id && e.Role == EmployeeRole.Advisor).Skip(skip).First().Id;
        long Driver(string branch, int skip = 0) => c.Employees.Where(e => e.BranchId == c.BranchByName(branch).Id && e.Role == EmployeeRole.Driver).Skip(skip).First().Id;
        var heredAdvisor = Advisor("Heredia");
        staff.Add(new StaffAssignment { ServiceId = ip1.Id, EmployeeId = heredAdvisor, StartsAt = ip1.ScheduledAt, EndsAt = ip1.ScheduledAt.AddHours(24), CreatedAt = now });
        staff.Add(new StaffAssignment { ServiceId = paidUrgent.Id, EmployeeId = heredAdvisor, StartsAt = paidUrgent.ScheduledAt, EndsAt = paidUrgent.ScheduledAt.AddHours(24), CreatedAt = now });
        alerts.Add(new Alert { Type = AlertType.StaffOverlap, BranchId = ip1.BranchId, ServiceId = paidUrgent.Id, CreatedAt = now.AddMinutes(-20),
            Message = $"El asesor asignado a {paidUrgent.Code} también atiende {ip1.Code} en el mismo horario." });
        foreach (var s in new[] { ip2, ip3, ip4, ip5, ip6, paidOpen })
            staff.Add(new StaffAssignment { ServiceId = s.Id, EmployeeId = Advisor(s.Branch!.Name), StartsAt = s.ScheduledAt, EndsAt = s.ScheduledAt.AddHours(24), CreatedAt = now });

        // ---- traslados
        Transfer NewTransfer(Service s, TransferStatus st, int driverSkip = 0)
        {
            var h = hearses.First(x => x.BranchId == s.BranchId && !usedHearses.Contains(x.Id));
            usedHearses.Add(h.Id);
            var t = new Transfer { ServiceId = s.Id, HearseId = h.Id, DriverId = Driver(s.Branch!.Name, driverSkip), Status = st, CreatedAt = now };
            transfers.Add(t);
            return t;
        }
        void Mile(Transfer t, MilestoneType type, double recAgoMin, double syncAgoMin, Service s, MilestoneSource src = MilestoneSource.Online, double dLat = 0, double dLng = 0) =>
            t.Milestones.Add(new TransferMilestone
            {
                Type = type, RecordedAt = now.AddMinutes(-recAgoMin), SyncedAt = now.AddMinutes(-syncAgoMin), Source = src, ClientUuid = Guid.NewGuid(),
                Lat = s.Branch!.Lat + dLat, Lng = s.Branch.Lng + dLng, CreatedAt = now.AddMinutes(-syncAgoMin)
            });

        var t1 = NewTransfer(ip3, TransferStatus.OnRoute);       // con demora
        Mile(t1, MilestoneType.Departure, 75, 75, ip3, dLat: 0.002, dLng: 0.002);
        Mile(t1, MilestoneType.Pickup, 50, 50, ip3, dLat: 0.012, dLng: -0.01);
        alerts.Add(new Alert { Type = AlertType.TransferDelay, BranchId = ip3.BranchId, ServiceId = ip3.Id, CreatedAt = now.AddMinutes(-8),
            Message = $"Traslado de {ip3.Code} con demora: 50 min sin registrar el siguiente hito." });
        var t2 = NewTransfer(ip4, TransferStatus.Done);          // marcado sin señal y sincronizado
        Mile(t2, MilestoneType.Departure, 50, 50, ip4, dLat: 0.003, dLng: 0.002);
        Mile(t2, MilestoneType.Pickup, 34, 4, ip4, MilestoneSource.Offline, 0.02, 0.015);
        Mile(t2, MilestoneType.ArrivalWake, 18, 4, ip4, MilestoneSource.Offline, 0.005, -0.004);
        var t3 = NewTransfer(ip1, TransferStatus.Done);
        Mile(t3, MilestoneType.Departure, 380, 380, ip1, dLat: 0.002); Mile(t3, MilestoneType.Pickup, 360, 360, ip1, dLat: 0.015, dLng: 0.01); Mile(t3, MilestoneType.ArrivalWake, 335, 335, ip1, dLat: 0.003);
        var t4 = NewTransfer(paidUrgent, TransferStatus.Planned);

        // ---- cola del crematorio central (6 cuerpos)
        var ops = c.Employees.Where(e => e.Role == EmployeeRole.CrematoriumOperator).ToList();
        Cremation Crem(Service s, int pos, CremationStatus st, double waitingAgoMin)
        {
            var cr = new Cremation { ServiceId = s.Id, QueuePosition = pos, Status = st, WaitingAt = now.AddMinutes(-waitingAgoMin), CreatedAt = now.AddMinutes(-waitingAgoMin) };
            if (st >= CremationStatus.InOven) { cr.InOvenAt = now.AddMinutes(-waitingAgoMin + 60); cr.OperatorId = ops[0].Id; }
            if (st >= CremationStatus.Packed) { cr.PackedAt = now.AddMinutes(-waitingAgoMin + 240); cr.OperatorId = ops[1].Id; }
            if (st == CremationStatus.Delivered) { cr.DeliveredAt = now.AddMinutes(-90); cr.ReceivedBy = s.FamilyContact!.FullName; cr.OperatorId = ops[2].Id; }
            cremations.Add(cr);
            return cr;
        }
        Crem(done1, 1, CremationStatus.Delivered, 2200);
        Crem(ip2, 2, CremationStatus.InOven, 300);
        Crem(ip5, 3, CremationStatus.Packed, 600);
        Crem(ip6, 4, CremationStatus.Waiting, 150);
        Crem(ip4, 5, CremationStatus.Waiting, 90);
        Crem(paidCrem, 6, CremationStatus.Waiting, 30);

        // ---- guardar todo (los componentes se sueltan para no reinsertarlos)
        foreach (var s in all) { s.Branch = null; foreach (var l in s.Lines) l.Component = null; }
        db.Services.AddRange(all);
        db.ChapelBookings.AddRange(bookings);
        db.ProviderOffers.AddRange(offers);
        db.Transfers.AddRange(transfers);
        db.Cremations.AddRange(cremations);
        db.StaffAssignments.AddRange(staff);
        db.AuditLog.AddRange(audit);
        db.OutboundMessages.AddRange(messages);
        db.SaveChanges();

        // alertas de stock bajo + solicitudes de reabastecimiento automáticas (RF-27, RF-28)
        var low = db.CasketStock.Where(s => s.Qty <= s.MinQty).ToList();
        var seq = 40;
        foreach (var s in low)
        {
            var model = c.Caskets.First(m => m.Id == s.CasketModelId);
            var branch = c.Branches.First(b => b.Id == s.BranchId);
            alerts.Add(new Alert { Type = AlertType.LowStock, BranchId = s.BranchId, CreatedAt = now.AddMinutes(-90),
                Message = $"El modelo {model.Name} ({model.Sku}) quedó en {s.Qty} unidad(es) en {branch.Name}; el mínimo es {s.MinQty}." });
            db.RestockRequests.Add(new Domain.Inventory.RestockRequest { BranchId = s.BranchId, CasketModelId = s.CasketModelId, Qty = 4, Status = RestockStatus.Requested, RequestedAt = now.AddMinutes(-89), ExternalNumber = $"BOD-{++seq:D6}", CreatedAt = now.AddMinutes(-89) });
        }
        db.Alerts.AddRange(alerts);
        db.SaveChanges();

        // hechos de carrozas
        var inUse = transfers.Where(t => t.Status != TransferStatus.Done).Select(t => t.HearseId).ToList();
        db.Hearses.Where(h => inUse.Contains(h.Id)).ExecuteUpdate(u => u.SetProperty(h => h.Status, HearseStatus.InUse));
        return all.Count;
    }
}
