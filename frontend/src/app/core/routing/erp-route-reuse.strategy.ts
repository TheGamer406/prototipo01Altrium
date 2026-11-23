/**
 * Archivo:    frontend/src/app/core/routing/erp-route-reuse.strategy.ts
 * Qué hace:   Reutiliza rutas de detalle de servicio (pestañas ERP).
 * Recibe:     Rutas con data.reuseKey = 'service-detail'.
 * Procesa:    Guarda el árbol del componente por id de servicio al salir y lo devuelve al volver; lo
 *               libera al cerrar la pestaña (OpenTabsService).
 * Devuelve:   DetachedRouteHandle por servicio.
 * Se conecta: ← app.config.ts · ← open-tabs.service.ts
 * Estado:     Completo
 */

import { Injectable } from '@angular/core';
import { ActivatedRouteSnapshot, DetachedRouteHandle, RouteReuseStrategy } from '@angular/router';

@Injectable({ providedIn: 'root' })
export class ErpRouteReuseStrategy implements RouteReuseStrategy {
  private stored = new Map<string, DetachedRouteHandle>();

  private key(route: ActivatedRouteSnapshot): string | null {
    return route.data?.['reuseKey'] === 'service-detail' ? `service:${route.params['id']}` : null;
  }

  shouldDetach(route: ActivatedRouteSnapshot): boolean { return this.key(route) !== null; }
  store(route: ActivatedRouteSnapshot, handle: DetachedRouteHandle | null): void {
    const k = this.key(route);
    if (k && handle) this.stored.set(k, handle);
  }
  shouldAttach(route: ActivatedRouteSnapshot): boolean { const k = this.key(route); return !!k && this.stored.has(k); }
  retrieve(route: ActivatedRouteSnapshot): DetachedRouteHandle | null { const k = this.key(route); return k ? this.stored.get(k) ?? null : null; }
  shouldReuseRoute(future: ActivatedRouteSnapshot, curr: ActivatedRouteSnapshot): boolean { return future.routeConfig === curr.routeConfig; }

  /** Libera el árbol guardado al cerrar la pestaña. */
  release(serviceId: string): void { this.stored.delete(`service:${serviceId}`); }
}
