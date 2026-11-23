/**
 * Archivo:    frontend/src/app/core/state/open-tabs.service.ts
 * Qué hace:   Pestañas ERP de servicios abiertos.
 * Recibe:     open(id, code) desde la lista o la búsqueda; close(id).
 * Procesa:    tabs = signal<{id, code, dirty}[]>; máximo 8; persiste en sessionStorage.
 * Devuelve:   tabs(), active(); erp-tabs-bar los muestra.
 * Se conecta: ← service-list, topbar (búsqueda) · → erp-tabs-bar, ErpRouteReuseStrategy (libera la
 *               ruta al cerrar)
 * Estado:     Pendiente
 */
