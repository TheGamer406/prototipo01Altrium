/**
 * Archivo:    frontend/src/app/core/http/demo-headers.interceptor.ts
 * Qué hace:   Agrega X-Demo-Role y X-Demo-Branch a cada petición /api.
 * Recibe:     HttpRequest saliente.
 * Procesa:    Lee RoleContextService y BranchContextService y clona la petición con los encabezados.
 * Devuelve:   HttpRequest modificada.
 * Se conecta: ← app.config.ts · → backend DemoUserMiddleware
 * Reglas:     RF-30
 * Estado:     Pendiente
 */
