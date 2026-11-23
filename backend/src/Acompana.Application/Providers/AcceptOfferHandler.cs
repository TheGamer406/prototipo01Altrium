/*
 * Archivo:    backend/src/Acompana.Application/Providers/AcceptOfferHandler.cs
 * Qué hace:   El proveedor acepta una oferta con un clic; gana el primero.
 * Recibe:     offerId + ICurrentUser (rol Proveedor, ProviderId).
 * Procesa:    en transacción: UPDATE service_lines SET provider_id=@p, status='Accepted',
 *               accepted_at=now(), accepted_price_crc=unit_price WHERE id=@line AND provider_id IS NULL
 *             si afectó 1 fila: oferta Won, demás ofertas de la línea Lost; notifica OfferAwarded
 *             si afectó 0 filas: oferta Lost
 * Devuelve:   AcceptOfferResult { won, message }.
 * Se conecta: ← POST /api/offers/{id}/accept, AdminController (simulación concurrente) · probado en
 *               SingleAwardTests
 * Reglas:     RF-16 · RNF-16
 * Estado:     Completo
 *
 * Contrato:
 *   Task<AcceptOfferResult> HandleAsync(Guid offerId, long providerId, CancellationToken ct)
 *     Recibe:     oferta y proveedor
 *     Procesa:    UPDATE condicional
 *     Devuelve:   ganó o 'Ya fue asignada'
 *     Errores:    404 · 410 'La oferta venció.'
 */

using Acompana.Application.Common;
using Acompana.Domain.Enums;
using Microsoft.EntityFrameworkCore;

namespace Acompana.Application.Providers;

public sealed class AcceptOfferHandler
{
    private readonly IAppDbContext _db;
    private readonly IOpsNotifier _notifier;
    public AcceptOfferHandler(IAppDbContext db, IOpsNotifier notifier) { _db = db; _notifier = notifier; }

    public async Task<AcceptOfferResult> HandleAsync(Guid offerId, long providerId, CancellationToken ct)
    {
        var offer = await _db.ProviderOffers.AsNoTracking().FirstOrDefaultAsync(o => o.Id == offerId && o.ProviderId == providerId, ct)
                    ?? throw new NotFoundException("La oferta no existe.");
        if (offer.Status == OfferStatus.Expired) throw new GoneException("La oferta venció.");
        if (offer.Status != OfferStatus.Open) return new AcceptOfferResult(false, "Ya fue asignada");

        var lineId = offer.ServiceLineId;
        int rows;
        await using (var tx = await _db.BeginTransactionAsync(ct))
        {
            // Adjudicación atómica: solo gana quien encuentre la línea sin proveedor (RNF-16)
            rows = await _db.ExecuteSqlAsync($@"UPDATE service_lines SET provider_id = {providerId}, status = 'Accepted', accepted_at = now(),
                accepted_price_crc = unit_price_crc, updated_at = now() WHERE id = {lineId} AND provider_id IS NULL", ct);
            if (rows == 1)
            {
                await _db.ExecuteSqlAsync($"UPDATE provider_offers SET status = 'Won', responded_at = now(), updated_at = now() WHERE id = {offerId}", ct);
                await _db.ExecuteSqlAsync($"UPDATE provider_offers SET status = 'Lost', responded_at = now(), updated_at = now() WHERE service_line_id = {lineId} AND id <> {offerId} AND status = 'Open'", ct);
            }
            else
                await _db.ExecuteSqlAsync($"UPDATE provider_offers SET status = 'Lost', responded_at = now(), updated_at = now() WHERE id = {offerId} AND status = 'Open'", ct);
            await tx.CommitAsync(ct);
        }

        if (rows == 0) return new AcceptOfferResult(false, "Ya fue asignada");

        var info = await (from l in _db.ServiceLines.AsNoTracking()
                          join s in _db.Services.AsNoTracking() on l.ServiceId equals s.Id
                          join p in _db.Providers.AsNoTracking() on l.ProviderId equals p.Id
                          where l.Id == lineId
                          select new { l.ServiceId, s.BranchId, Provider = p.Name, Price = l.AcceptedPriceCrc ?? l.UnitPriceCrc, l.AcceptedAt }).FirstAsync(ct);
        await _notifier.OfferAwarded(lineId, info.Provider, info.Price, info.AcceptedAt ?? DateTimeOffset.UtcNow, info.BranchId);
        await _notifier.ServiceUpdated(info.ServiceId, info.BranchId);
        return new AcceptOfferResult(true, "Adjudicado");
    }
}
