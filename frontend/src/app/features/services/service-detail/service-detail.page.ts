/**
 * Archivo:    frontend/src/app/features/services/service-detail/service-detail.page.ts
 * Qué hace:   Detalle del servicio con pestañas internas.
 * Recibe:     Ruta :id.
 * Procesa:    Carga ServiceDetailDto; registra la pestaña ERP; escucha ServiceUpdated del hub para
 *               recargar.
 * Devuelve:   Encabezado (código, estado, sede) + pestañas Resumen · Paquete · Documentos · Factura ·
 *               Proveedores · Logística · Mensajes · Bitácora.
 * Se conecta: → ServicesApi, OpenTabsService, OpsHubService
 * Reglas:     RF-01 a RF-13 · RF-19
 * Estado:     Pendiente
 */
