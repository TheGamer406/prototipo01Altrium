/**
 * Archivo:    frontend/src/app/core/layout/topbar.component.ts
 * Qué hace:   Barra superior: logo, migas, búsqueda global, sede, rol, alertas, modo admin.
 * Recibe:     Estado de los servicios de contexto + alertas del hub.
 * Procesa:    Búsqueda con debounce (300 ms) a GET /api/services?q=; abre el servicio como pestaña
 *               ERP.
 * Devuelve:   Cambios de sede/rol y navegación.
 * Se conecta: ← shell · → branch/role/admin-mode services, ServicesApi, AlertsApi
 * Reglas:     RF-29 · RF-30
 * Estado:     Pendiente
 */
