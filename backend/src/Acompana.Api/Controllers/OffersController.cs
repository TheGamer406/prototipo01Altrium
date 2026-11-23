/*
 * Archivo:    backend/src/Acompana.Api/Controllers/OffersController.cs
 * Qué hace:   Bandeja y aceptación de ofertas del proveedor.
 * Recibe:     Peticiones HTTP bajo /api con X-Demo-Role y X-Demo-Branch (ya resueltos en
 *               ICurrentUser).
 * Procesa:    Sin lógica de negocio: valida forma, llama al handler/query y traduce el resultado a
 *               HTTP.
 * Devuelve:   JSON camelCase; errores como ProblemDetails con mensaje en español.
 * Se conecta: → Application/Providers/GetProviderInboxQuery, AcceptOfferHandler
 * Reglas:     RF-15 · RF-16 · RNF-16
 * Estado:     Pendiente
 *
 * Contrato:
 *   GET /api/offers?providerId=  (RF-15)
 *     Recibe:     proveedor
 *     Devuelve:   200 { open[], history[] }
 *   POST /api/offers/{id}/accept  (RF-16)
 *     Recibe:     id de la oferta (proveedor desde ICurrentUser)
 *     Devuelve:   200 AcceptOfferResult { won, message }
 */
