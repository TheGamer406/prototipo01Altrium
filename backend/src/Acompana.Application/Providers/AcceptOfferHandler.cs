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
 * Estado:     Pendiente
 *
 * Contrato:
 *   Task<AcceptOfferResult> HandleAsync(Guid offerId, long providerId, CancellationToken ct)
 *     Recibe:     oferta y proveedor
 *     Procesa:    UPDATE condicional
 *     Devuelve:   ganó o 'Ya fue asignada'
 *     Errores:    404 · 410 'La oferta venció.'
 */
