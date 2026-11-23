/**
 * Archivo:    frontend/src/app/core/layout/sidebar.component.ts
 * Qué hace:   Barra lateral plegable (248 → 64 px).
 * Recibe:     NAV + rol actual.
 * Procesa:    Filtra ítems por rol; colapso guardado en localStorage; tooltip cuando está plegada.
 * Devuelve:   Menú con ítem activo (barra de acento + --accent-soft).
 * Se conecta: ← shell · → nav-items.ts
 * Reglas:     RF-30
 * Estado:     Completo
 */

import { Component, computed, inject } from '@angular/core';
import { RouterLink, RouterLinkActive } from '@angular/router';
import { MatTooltipModule } from '@angular/material/tooltip';
import { NAV } from '../navigation/nav-items';
import { RoleContextService } from '../state/role-context.service';
import { LayoutService } from '../state/layout.service';

@Component({
  selector: 'app-sidebar',
  standalone: true,
  imports: [RouterLink, RouterLinkActive, MatTooltipModule],
  template: `
    <nav aria-label="Navegación principal">
      @for (section of sections(); track section.title) {
        @if (!layout.collapsed()) { <div class="title">{{ section.title }}</div> } @else { <div class="sep"></div> }
        @for (item of section.items; track item.route) {
          <a [routerLink]="item.route" routerLinkActive="active" [matTooltip]="layout.collapsed() ? item.label : ''" matTooltipPosition="right">
            <span class="ms">{{ item.icon }}</span>
            @if (!layout.collapsed()) { <span class="label">{{ item.label }}</span> }
          </a>
        }
      }
    </nav>
    <button class="toggle btn ghost" type="button" (click)="layout.toggle()" [attr.aria-label]="layout.collapsed() ? 'Expandir menú' : 'Plegar menú'">
      <span class="ms">{{ layout.collapsed() ? 'right_panel_open' : 'left_panel_close' }}</span>
    </button>`,
  styles: [`
    :host { display: flex; flex-direction: column; justify-content: space-between; height: 100%; padding: 8px; }
    nav { display: flex; flex-direction: column; gap: 2px; overflow-y: auto; }
    .title { font-size: 11px; text-transform: uppercase; letter-spacing: 0.6px; color: var(--text-muted); padding: 14px 10px 4px; }
    .sep { height: 1px; background: var(--border); margin: 8px 4px; }
    a { position: relative; display: flex; align-items: center; gap: 12px; height: 36px; padding: 0 10px; border-radius: var(--radius);
        color: var(--text); text-decoration: none; transition: background var(--transition); }
    a:hover { background: var(--surface-2); text-decoration: none; }
    a.active { background: var(--accent-soft); color: var(--accent-text); }
    a.active::before { content: ''; position: absolute; left: -8px; top: 6px; bottom: 6px; width: 3px; border-radius: 2px; background: var(--accent); }
    .label { white-space: nowrap; overflow: hidden; text-overflow: ellipsis; }
    .toggle { align-self: flex-start; }`],
})
export class SidebarComponent {
  private role = inject(RoleContextService);
  readonly layout = inject(LayoutService);
  readonly sections = computed(() =>
    NAV.map(s => ({ title: s.title, items: s.items.filter(i => i.roles.includes(this.role.role())) })).filter(s => s.items.length > 0));
}
