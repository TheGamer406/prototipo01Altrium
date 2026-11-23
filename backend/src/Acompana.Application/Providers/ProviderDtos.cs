/*
 * Archivo:    backend/src/Acompana.Application/Providers/ProviderDtos.cs
 * Qué hace:   DTO de proveedores y ofertas.
 * Recibe:     —
 * Procesa:    —
 * Devuelve:   ProviderDto { id, name, type, zone, email, phone, active, wonLast30d }
 *             OfferDto { id, serviceCode, componentName, category, place, scheduledAt, fixedPriceCrc,
 *               status, sentAt, expiresAt }
 *             OpenOpportunityDto { lineId, serviceCode, componentName, offersSent, minutesOpen,
 *               isOverdue }
 *             AcceptOfferResult { won: bool, message ('Adjudicado' | 'Ya fue asignada') }
 * Se conecta: ← ProvidersController, OffersController · → core/models/providers.models.ts
 * Reglas:     RF-14 a RF-17
 * Estado:     Pendiente
 */
