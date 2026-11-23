/*
 * Archivo:    backend/src/Acompana.Application/Providers/DispatchOffersHandler.cs
 * Qué hace:   Al pagar, envía ofertas a todos los proveedores activos del tipo y zona de cada línea.
 * Recibe:     serviceId (lo llama RegisterPaymentHandler).
 * Procesa:    por cada línea con RequiresProvider: proveedores activos con Type = ProviderType y Zone
 *               = zona de la sede
 *             crea ProviderOffer(Open) por proveedor; línea → Offered; DueAt = ScheduledAt − 2 h
 *             correo ProviderOffer con lugar, hora y precio fijo
 * Devuelve:   int ofertas creadas.
 * Se conecta: ← RegisterPaymentHandler · → IMailClient, IOpsNotifier (bandeja del proveedor se
 *               actualiza)
 * Reglas:     RF-15
 * Estado:     Pendiente
 *
 * Contrato:
 *   Task<int> HandleAsync(Guid serviceId, CancellationToken ct)
 *     Recibe:     servicio pagado
 *     Procesa:    ver arriba
 *     Devuelve:   cantidad de ofertas
 */
