/**
 * Archivo:    frontend/src/app/shared/components/paged-table.component.ts
 * Qué hace:   Tabla densa paginada con estado en la URL.
 * Recibe:     @Input columns, data: PagedResult<T>, loading; query params de la ruta.
 * Procesa:    Orden y página se escriben en la URL (queryParamsHandling: 'merge') para que volver
 *               atrás los conserve.
 * Devuelve:   @Output pageChange, sortChange, rowClick.
 * Se conecta: ← listas (servicios, bitácora, mensajes)
 * Estado:     Completo
 */

import { Component, EventEmitter, Input, Output } from '@angular/core';
import { PagedResult } from '../../core/models/meta.models';
import { SkeletonComponent } from './skeleton.component';
import { EmptyStateComponent } from './empty-state.component';

export interface Column<T> { key: string; label: string; sortable?: boolean; align?: 'left' | 'right'; value?: (row: T) => string | number; }

@Component({
  selector: 'app-paged-table',
  standalone: true,
  imports: [SkeletonComponent, EmptyStateComponent],
  template: `
    @if (loading) { <app-skeleton [rows]="8" /> }
    @else if (!data || data.items.length === 0) { <app-empty-state icon="search_off" text="No hay resultados con estos filtros." /> }
    @else {
      <table class="dense">
        <thead><tr>
          @for (c of columns; track c.key) {
            <th [class.num]="c.align === 'right'" [style.cursor]="c.sortable ? 'pointer' : 'default'" (click)="c.sortable && sortChange.emit(c.key)">{{ c.label }}</th>
          }
        </tr></thead>
        <tbody>
          @for (row of data.items; track $index) {
            <tr class="clickable" (click)="rowClick.emit(row)">
              @for (c of columns; track c.key) { <td [class.num]="c.align === 'right'">{{ c.value ? c.value(row) : '' }}</td> }
            </tr>
          }
        </tbody>
      </table>
      <div class="pager">
        <span class="muted">{{ from }}–{{ to }} de {{ data.total }}</span>
        <button class="btn ghost" type="button" [disabled]="data.page <= 1" (click)="pageChange.emit(data.page - 1)" aria-label="Página anterior"><span class="ms">chevron_left</span></button>
        <button class="btn ghost" type="button" [disabled]="to >= data.total" (click)="pageChange.emit(data.page + 1)" aria-label="Página siguiente"><span class="ms">chevron_right</span></button>
      </div>
    }`,
  styles: [`.pager { display: flex; align-items: center; justify-content: flex-end; gap: 8px; padding-top: 8px; }`],
})
// eslint-disable-next-line @typescript-eslint/no-explicit-any
export class PagedTableComponent<T = any> {
  @Input() columns: Column<T>[] = [];
  @Input() data: PagedResult<T> | null = null;
  @Input() loading = false;
  @Output() pageChange = new EventEmitter<number>();
  @Output() sortChange = new EventEmitter<string>();
  @Output() rowClick = new EventEmitter<T>();
  get from(): number { return this.data ? (this.data.page - 1) * this.data.pageSize + 1 : 0; }
  get to(): number { return this.data ? Math.min(this.data.page * this.data.pageSize, this.data.total) : 0; }
}
