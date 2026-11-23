/**
 * Archivo:    frontend/src/app/features/central/central.routes.ts
 * Qué hace:   Rutas de Vista central: sedes · servicios en curso · capillas · inventario.
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
  selector: 'app-central-shell',
  standalone: true,
  imports: [RouterOutlet, RouterLink, RouterLinkActive, PageHeaderComponent],
  template: `<app-page-header title="Vista central" description="Todas las sedes: servicios en curso, capillas ocupadas e inventario." [rf]="['RF-29']" />
    <div class="tabs"><a routerLink="mapa" routerLinkActive="on">Mapa</a><a routerLink="en-curso" routerLinkActive="on">Servicios en curso</a><a routerLink="capillas" routerLinkActive="on">Capillas</a><a routerLink="inventario" routerLinkActive="on">Inventario</a></div><router-outlet />`,
  styles: [`.tabs { display: flex; gap: 2px; border-bottom: 1px solid var(--border); margin-bottom: 16px; } .tabs a { color: var(--text-muted); padding: 8px 14px; border-bottom: 2px solid transparent; text-decoration: none; } .tabs .on { color: var(--text); border-color: var(--accent); }`],
})
export class CentralShell {}

export const centralRoutes: Routes = [{
  path: '', component: CentralShell, children: [
    { path: '', redirectTo: 'mapa', pathMatch: 'full' },
    { path: 'mapa', loadComponent: () => import('./branches-map.page').then(m => m.BranchesMapPage) },
    { path: 'en-curso', loadComponent: () => import('./ongoing-services.page').then(m => m.OngoingServicesPage) },
    { path: 'capillas', loadComponent: () => import('./chapels-overview.page').then(m => m.ChapelsOverviewPage) },
    { path: 'inventario', loadComponent: () => import('./inventory-overview.page').then(m => m.InventoryOverviewPage) },
  ],
}];
