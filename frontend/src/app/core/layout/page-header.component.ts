/**
 * Archivo:    frontend/src/app/core/layout/page-header.component.ts
 * Qué hace:   Encabezado de página estándar.
 * Recibe:     @Input title, description, rf (string[]); ng-content para acción principal y pestañas.
 * Procesa:    —
 * Devuelve:   Título + descripción + etiqueta RF + acción a la derecha.
 * Se conecta: ← todas las páginas
 * Estado:     Completo
 */

import { Component, Input } from '@angular/core';
import { RfTagComponent } from '../../shared/components/rf-tag.component';

@Component({
  selector: 'app-page-header',
  standalone: true,
  imports: [RfTagComponent],
  template: `
    <header class="ph">
      <div class="text">
        <div class="row"><h1>{{ title }}</h1><app-rf-tag [rf]="rf" /></div>
        @if (description) { <p class="muted">{{ description }}</p> }
      </div>
      <div class="actions"><ng-content /></div>
    </header>`,
  styles: [`.ph { flex-wrap: wrap; display: flex; align-items: flex-start; justify-content: space-between; gap: 16px; margin-bottom: 16px; }
    .row { display: flex; align-items: center; gap: 10px; } p { margin: 4px 0 0; } .actions { display: flex; gap: 8px; align-items: center; }`],
})
export class PageHeaderComponent {
  @Input() title = '';
  @Input() description = '';
  @Input() rf: string[] = [];
}
