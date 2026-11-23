/**
 * Archivo:    frontend/src/app/core/routing/erp-route-reuse.strategy.ts
 * Qué hace:   Reutiliza rutas de detalle de servicio (pestañas ERP).
 * Recibe:     Rutas con data.reuseKey = 'service-detail'.
 * Procesa:    Guarda el árbol del componente por id de servicio al salir y lo devuelve al volver; lo
 *               libera al cerrar la pestaña (OpenTabsService).
 * Devuelve:   DetachedRouteHandle por servicio.
 * Se conecta: ← app.config.ts · ← open-tabs.service.ts
 * Estado:     Pendiente
 */
