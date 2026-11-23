/*
 * Archivo:    backend/src/Acompana.Domain/Providers/ProviderOffer.cs
 * Qué hace:   Entidad ProviderOffer, se guarda en la tabla `provider_offers`.
 * Recibe:     Campos: ServiceLineId, ProviderId, SentAt, Status (OfferStatus), RespondedAt?. Los llena
 *               el handler que la crea o EF Core al leer.
 * Procesa:    Open → Won | Lost | Expired. Solo una oferta Won por línea.
 * Devuelve:   Nada; es estado. Hereda Id, CreatedAt y UpdatedAt de Entity.
 * Se conecta: ← DispatchOffersHandler, AcceptOfferHandler, OfferTimeoutChecker
 * Reglas:     RF-15 · RF-16 · RF-17
 * Estado:     Pendiente
 */
