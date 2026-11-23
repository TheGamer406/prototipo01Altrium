/**
 * Archivo:    frontend/src/app/features/services/service-detail/service-board.component.ts
 * Qué hace:   Tablero del servicio: estado y responsable de cada componente.
 * Recibe:     @Input serviceId.
 * Procesa:    GET board; se actualiza con OfferAwarded/ServiceUpdated; urgentes (< 2 h) resaltados.
 * Devuelve:   Tarjetas por componente con chip de estado, proveedor, precio y hora.
 * Se conecta: ← detail-summary, home · → ServicesApi, OpsHubService
 * Reglas:     RF-17 · RF-19
 * Estado:     Pendiente
 */
