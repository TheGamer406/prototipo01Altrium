/**
 * Archivo:    frontend/src/app/features/catalog/catalog.routes.ts
 * Qué hace:   Rutas: paquetes · componentes · modelos de cofre.
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
  selector: 'app-catalog-shell',
  standalone: true,
  imports: [RouterOutlet, RouterLink, RouterLinkActive, PageHeaderComponent],
  template: `<app-page-header title="Catálogo" description="Componentes, paquetes y modelos de cofre editables sin programar." [rf]="['RF-04','RF-06','RF-10']" />
    <div class="tabs"><a routerLink="componentes" routerLinkActive="on">Componentes</a><a routerLink="paquetes" routerLinkActive="on">Paquetes</a><a routerLink="cofres" routerLinkActive="on">Cofres</a></div><router-outlet />`,
  styles: [`.tabs { display: flex; gap: 2px; border-bottom: 1px solid var(--border); margin-bottom: 16px; } .tabs a { color: var(--text-muted); padding: 8px 14px; border-bottom: 2px solid transparent; text-decoration: none; } .tabs .on { color: var(--text); border-color: var(--accent); }`],
})
export class CatalogShell {}

export const catalogRoutes: Routes = [{
  path: '', component: CatalogShell, children: [
    { path: '', redirectTo: 'componentes', pathMatch: 'full' },
    { path: 'componentes', loadComponent: () => import('./components.page').then(m => m.ComponentsPage) },
    { path: 'paquetes', loadComponent: () => import('./packages.page').then(m => m.PackagesPage) },
    { path: 'cofres', loadComponent: () => import('./casket-models.page').then(m => m.CasketModelsPage) },
  ],
}];
