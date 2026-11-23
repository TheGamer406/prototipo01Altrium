/*
 * Archivo:    backend/src/Acompana.Application/Providers/GetProviderInboxQuery.cs
 * Qué hace:   Bandeja del proveedor: ofertas abiertas e historial.
 * Recibe:     providerId (del rol Proveedor o query string en modo admin).
 * Procesa:    Ofertas del proveedor; abiertas primero, ordenadas por vencimiento.
 * Devuelve:   { open: OfferDto[], history: OfferDto[] }.
 * Se conecta: ← GET /api/offers?providerId=
 * Reglas:     RF-15 · RF-16
 * Estado:     Pendiente
 */
