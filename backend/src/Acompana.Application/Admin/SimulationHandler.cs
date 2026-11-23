/*
 * Archivo:    backend/src/Acompana.Application/Admin/SimulationHandler.cs
 * Qué hace:   Simulaciones del modo admin para el demo.
 * Recibe:     tipo de simulación y parámetros.
 * Procesa:    ConcurrentAccept: elige una línea Offered con ≥ 2 ofertas y lanza dos AcceptOfferHandler
 *               en paralelo (Task.WhenAll)
 *             NextInvoiceReject: IMockControlClient.RejectNextInvoiceAsync
 *             CoreDown(minutes): IMockControlClient.SetCoreDownAsync
 * Devuelve:   { description, results[] } para mostrar en un snackbar.
 * Se conecta: ← POST /api/admin/simulate/* · → AcceptOfferHandler, IMockControlClient
 * Reglas:     RF-16 · RF-13
 * Estado:     Completo
 */

using Acompana.Application.Common;
using Acompana.Application.Integrations;
using Acompana.Application.Providers;
using Acompana.Domain.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Acompana.Application.Admin;

public record SimulationResult(string Description, IReadOnlyList<string> Results);

public sealed class SimulationHandler
{
    private readonly IAppDbContext _db;
    private readonly IServiceScopeFactory _scopes;
    private readonly IMockControlClient _mock;
    public SimulationHandler(IAppDbContext db, IServiceScopeFactory scopes, IMockControlClient mock) { _db = db; _scopes = scopes; _mock = mock; }

    /// <summary>Dos proveedores aceptan la misma línea a la vez; gana exactamente uno (RF-16, RNF-16).</summary>
    public async Task<SimulationResult> ConcurrentAcceptAsync(CancellationToken ct)
    {
        var pair = await (from o in _db.ProviderOffers.AsNoTracking()
                          where o.Status == OfferStatus.Open
                          group o by o.ServiceLineId into g
                          where g.Count() >= 2
                          select new { Line = g.Key, Offers = g.OrderBy(x => x.ProviderId).Take(2).Select(x => new { x.Id, x.ProviderId }).ToList() })
                         .FirstOrDefaultAsync(ct);
        if (pair is null) throw new ConflictException("No hay una línea con ofertas abiertas para simular. Registre un pago primero.");

        async Task<AcceptOfferResult> Accept(long providerId, Guid offerId)
        {
            using var scope = _scopes.CreateScope();
            return await scope.ServiceProvider.GetRequiredService<AcceptOfferHandler>().HandleAsync(offerId, providerId, ct);
        }
        var results = await Task.WhenAll(pair.Offers.Select(o => Accept(o.ProviderId, o.Id)));
        var names = await _db.Providers.AsNoTracking().Where(p => pair.Offers.Select(o => o.ProviderId).Contains(p.Id)).ToDictionaryAsync(p => p.Id, p => p.Name, ct);
        var lines = pair.Offers.Select((o, i) => $"{names[o.ProviderId]}: {results[i].Message}").ToList();
        return new SimulationResult("Dos proveedores aceptaron al mismo tiempo", lines);
    }

    public async Task<SimulationResult> NextInvoiceRejectAsync(CancellationToken ct)
    {
        await _mock.RejectNextInvoiceAsync(ct);
        return new SimulationResult("La próxima factura será rechazada por el core contable", Array.Empty<string>());
    }

    public async Task<SimulationResult> CoreDownAsync(int minutes, CancellationToken ct)
    {
        await _mock.SetCoreDownAsync(minutes, ct);
        return new SimulationResult($"El core contable queda sin respuesta por {minutes} min", Array.Empty<string>());
    }
}
