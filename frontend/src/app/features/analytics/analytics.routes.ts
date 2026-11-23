/**
 * Archivo:    frontend/src/app/features/analytics/analytics.routes.ts
 * Qué hace:   Rutas: ventas · eficiencia · zonas y temporada.
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
  selector: 'app-analytics-shell',
  standalone: true,
  imports: [RouterOutlet, RouterLink, RouterLinkActive, PageHeaderComponent],
  template: `<app-page-header title="Analítica" description="Ventas, eficiencia por etapa y servicios por zona (almacén analítico separado de la operación)." [rf]="['RF-32','RF-33','RF-34']" />
    <div class="tabs"><a routerLink="ventas" routerLinkActive="on">Ventas</a><a routerLink="eficiencia" routerLinkActive="on">Eficiencia</a><a routerLink="zonas" routerLinkActive="on">Zonas</a></div><router-outlet />`,
  styles: [`.tabs { display: flex; gap: 2px; border-bottom: 1px solid var(--border); margin-bottom: 16px; } .tabs a { color: var(--text-muted); padding: 8px 14px; border-bottom: 2px solid transparent; text-decoration: none; } .tabs .on { color: var(--text); border-color: var(--accent); }`],
})
export class AnalyticsShell {}

export const analyticsRoutes: Routes = [{
  path: '', component: AnalyticsShell, children: [
    { path: '', redirectTo: 'ventas', pathMatch: 'full' },
    { path: 'ventas', loadComponent: () => import('./sales.page').then(m => m.SalesPage) },
    { path: 'eficiencia', loadComponent: () => import('./efficiency.page').then(m => m.EfficiencyPage) },
    { path: 'zonas', loadComponent: () => import('./zones.page').then(m => m.ZonesPage) },
  ],
}];
