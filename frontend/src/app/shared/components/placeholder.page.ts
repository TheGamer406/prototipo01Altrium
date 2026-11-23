/**
 * Archivo:    frontend/src/app/shared/components/placeholder.page.ts
 * Qué hace:   Pantalla provisional de un módulo que aún no se implementa (fases 4 a 6).
 * Recibe:     data de la ruta { title, description, rf }.
 * Procesa:    Lee la ruta activa y pinta encabezado de página + estado vacío.
 * Devuelve:   Encabezado y mensaje 'Esta pantalla se habilita en una fase posterior.'
 * Se conecta: ← app.routes.ts · → PageHeaderComponent, EmptyStateComponent
 * Estado:     Completo
 */
import { Component, inject } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { PageHeaderComponent } from '../../core/layout/page-header.component';
import { EmptyStateComponent } from './empty-state.component';

@Component({
  selector: 'app-placeholder-page',
  standalone: true,
  imports: [PageHeaderComponent, EmptyStateComponent],
  template: `
    <app-page-header [title]="data['title']" [description]="data['description'] ?? ''" [rf]="data['rf'] ?? []" />
    <div class="card"><app-empty-state icon="construction" text="Esta pantalla se habilita en una fase posterior del prototipo." /></div>`,
})
export class PlaceholderPage { data = inject(ActivatedRoute).snapshot.data; }
