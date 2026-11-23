/**
 * Archivo:    frontend/src/app/core/layout/shell.component.ts
 * Qué hace:   Estructura de la app: barra lateral, barra superior, pestañas ERP y contenido.
 * Recibe:     —
 * Procesa:    Grid de 248/64 px + 56 px; banda ámbar si modo admin.
 * Devuelve:   Layout con <router-outlet>.
 * Se conecta: ← app.component · → sidebar, topbar, erp-tabs-bar, footer
 * Estado:     Completo
 */

import { Component, inject } from '@angular/core';
import { RouterOutlet } from '@angular/router';
import { AdminModeService } from '../state/admin-mode.service';
import { LayoutService } from '../state/layout.service';
import { SidebarComponent } from './sidebar.component';
import { TopbarComponent } from './topbar.component';
import { ErpTabsBarComponent } from './erp-tabs-bar.component';
import { FooterComponent } from './footer.component';

@Component({
  selector: 'app-shell',
  standalone: true,
  imports: [RouterOutlet, SidebarComponent, TopbarComponent, ErpTabsBarComponent, FooterComponent],
  template: `
    @if (admin.enabled()) { <div class="admin-band" role="status">Modo admin del demo: las simulaciones y el reinicio están disponibles en el menú Admin.</div> }
    <div class="shell" [class.collapsed]="layout.collapsed()">
      <app-sidebar class="side" />
      <app-topbar class="top" />
      <main class="main">
        <app-erp-tabs-bar />
        <div class="content"><router-outlet /></div>
        <app-footer />
      </main>
    </div>`,
  styles: [`
    :host { display: flex; flex-direction: column; height: 100vh; }
    .admin-band { height: 24px; background: #5b4a1f; color: #f1dfb0; font-size: 12px; display: grid; place-items: center; }
    .shell { display: grid; flex: 1; min-height: 0; grid-template-columns: var(--sidebar-w) 1fr; grid-template-rows: var(--topbar-h) 1fr; transition: grid-template-columns var(--transition); }
    .shell.collapsed { grid-template-columns: var(--sidebar-w-collapsed) 1fr; }
    .side { grid-row: 2; grid-column: 1; background: var(--surface); border-right: 1px solid var(--border); overflow: hidden; }
    .top { grid-row: 1; grid-column: 1 / 3; background: var(--surface); border-bottom: 1px solid var(--border); }
    .main { grid-row: 2; grid-column: 2; display: flex; flex-direction: column; min-height: 0; }
    .content { flex: 1; overflow: auto; padding: 24px; }`],
})
export class ShellComponent {
  readonly admin = inject(AdminModeService);
  readonly layout = inject(LayoutService);
}
