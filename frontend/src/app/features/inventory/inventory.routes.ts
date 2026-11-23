/**
 * Archivo:    frontend/src/app/features/inventory/inventory.routes.ts
 * Qué hace:   Rutas: stock · alertas · reabastecimiento.
 * Recibe:     —
 * Procesa:    —
 * Devuelve:   Routes.
 * Se conecta: ← app.routes.ts
 * Estado:     Completo
 */

import { Component } from '@angular/core';
import { RouterLink, RouterLinkActive, RouterOutlet, Routes } from '@angular/router';
import { PageHeaderComponent } from '../../core/layout/page-header.component';

@Component({
  selector: 'app-inventory-shell',
  standalone: true,
  imports: [RouterOutlet, RouterLink, RouterLinkActive, PageHeaderComponent],
  template: `<app-page-header title="Inventario de cofres" description="Existencias por sede, alertas de mínimo y solicitudes a la bodega central." [rf]="['RF-26','RF-27','RF-28']" />
    <div class="tabs"><a routerLink="existencias" routerLinkActive="on">Existencias</a><a routerLink="alertas" routerLinkActive="on">Alertas</a><a routerLink="solicitudes" routerLinkActive="on">Solicitudes</a></div><router-outlet />`,
  styles: [`.tabs { display: flex; gap: 2px; border-bottom: 1px solid var(--border); margin-bottom: 16px; } .tabs a { color: var(--text-muted); padding: 8px 14px; border-bottom: 2px solid transparent; text-decoration: none; } .tabs .on { color: var(--text); border-color: var(--accent); }`],
})
export class InventoryShell {}

export const inventoryRoutes: Routes = [{
  path: '', component: InventoryShell, children: [
    { path: '', redirectTo: 'existencias', pathMatch: 'full' },
    { path: 'existencias', loadComponent: () => import('./stock.page').then(m => m.StockPage) },
    { path: 'alertas', loadComponent: () => import('./inventory-alerts.page').then(m => m.InventoryAlertsPage) },
    { path: 'solicitudes', loadComponent: () => import('./restock-requests.page').then(m => m.RestockRequestsPage) },
  ],
}];
