/**
 * Archivo:    frontend/src/app/features/providers/providers.routes.ts
 * Qué hace:   Rutas: oportunidades abiertas · cartera.
 * Recibe:     —
 * Procesa:    Pestañas internas.
 * Devuelve:   Routes.
 * Se conecta: ← app.routes.ts
 * Estado:     Completo
 */

import { Component } from '@angular/core';
import { RouterLink, RouterLinkActive, RouterOutlet, Routes } from '@angular/router';
import { PageHeaderComponent } from '../../core/layout/page-header.component';

@Component({
  selector: 'app-providers-shell',
  standalone: true,
  imports: [RouterOutlet, RouterLink, RouterLinkActive, PageHeaderComponent],
  template: `<app-page-header title="Proveedores" description="Oportunidades abiertas y cartera de proveedores." [rf]="['RF-14','RF-17']" />
    <div class="tabs"><a routerLink="oportunidades" routerLinkActive="on">Oportunidades abiertas</a><a routerLink="cartera" routerLinkActive="on">Cartera</a></div><router-outlet />`,
  styles: [`.tabs { display: flex; gap: 2px; border-bottom: 1px solid var(--border); margin-bottom: 16px; } .tabs a { color: var(--text-muted); padding: 8px 14px; border-bottom: 2px solid transparent; text-decoration: none; } .tabs .on { color: var(--text); border-color: var(--accent); }`],
})
export class ProvidersShell {}

export const providersRoutes: Routes = [{
  path: '', component: ProvidersShell, children: [
    { path: '', redirectTo: 'oportunidades', pathMatch: 'full' },
    { path: 'oportunidades', loadComponent: () => import('./open-opportunities.page').then(m => m.OpenOpportunitiesPage) },
    { path: 'cartera', loadComponent: () => import('./provider-portfolio.page').then(m => m.ProviderPortfolioPage) },
  ],
}];
