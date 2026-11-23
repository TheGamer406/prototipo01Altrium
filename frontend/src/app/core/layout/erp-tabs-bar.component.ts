/**
 * Archivo:    frontend/src/app/core/layout/erp-tabs-bar.component.ts
 * Qué hace:   Fila de pestañas de servicios abiertos.
 * Recibe:     OpenTabsService.tabs().
 * Procesa:    Clic cambia de pestaña; × cierra (pide confirmar si dirty).
 * Devuelve:   Navegación.
 * Se conecta: ← shell · → open-tabs.service.ts
 * Estado:     Completo
 */

import { Component, inject } from '@angular/core';
import { Router } from '@angular/router';
import { MatDialog } from '@angular/material/dialog';
import { OpenTabsService } from '../state/open-tabs.service';
import { ConfirmDialogComponent } from '../../shared/components/confirm-dialog.component';

@Component({
  selector: 'app-erp-tabs-bar',
  standalone: true,
  template: `
    @if (tabs.tabs().length) {
      <div class="bar" role="tablist" aria-label="Servicios abiertos">
        @for (t of tabs.tabs(); track t.id) {
          <div class="tab" role="tab" [class.active]="isActive(t.id)" [attr.aria-selected]="isActive(t.id)">
            <button type="button" class="name" (click)="go(t.id)">{{ t.code }}@if (t.dirty) { <i title="Cambios sin guardar"></i> }</button>
            <button type="button" class="x" (click)="close(t.id, t.dirty)" [attr.aria-label]="'Cerrar ' + t.code"><span class="ms">close</span></button>
          </div>
        }
      </div>
    }`,
  styles: [`
    .bar { display: flex; gap: 4px; padding: 6px 24px 0; border-bottom: 1px solid var(--border); overflow-x: auto; }
    .tab { display: flex; align-items: center; border: 1px solid var(--border); border-bottom: none; border-radius: var(--radius) var(--radius) 0 0; background: var(--surface); }
    .tab.active { background: var(--bg); border-color: var(--accent); }
    button { background: none; border: none; color: var(--text); font: inherit; cursor: pointer; height: 30px; }
    .name { padding: 0 8px 0 12px; display: flex; align-items: center; gap: 6px; } .x { padding: 0 6px; color: var(--text-muted); } .x .ms { font-size: 16px; }
    i { width: 6px; height: 6px; border-radius: 50%; background: var(--warning); }`],
})
export class ErpTabsBarComponent {
  readonly tabs = inject(OpenTabsService);
  private router = inject(Router);
  private dialog = inject(MatDialog);
  isActive(id: string): boolean { return this.router.url.startsWith(`/servicios/${id}`); }
  go(id: string): void { void this.router.navigate(['/servicios', id]); }
  close(id: string, dirty: boolean): void {
    if (!dirty) return this.tabs.close(id);
    this.dialog.open(ConfirmDialogComponent, { data: { title: 'Cambios sin guardar', message: 'Esta pestaña tiene cambios sin guardar. ¿Cerrarla de todos modos?', confirmLabel: 'Cerrar', danger: true } })
      .afterClosed().subscribe(ok => ok && this.tabs.close(id));
  }
}
