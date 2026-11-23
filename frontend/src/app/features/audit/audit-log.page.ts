/**
 * Archivo:    frontend/src/app/features/audit/audit-log.page.ts
 * Qué hace:   Bitácora filtrable.
 * Recibe:     Filtros en la URL.
 * Procesa:    Tabla paginada; expandir fila muestra valor anterior y nuevo (diff de jsonb).
 * Devuelve:   Tabla.
 * Se conecta: → OversightApi
 * Reglas:     RF-31
 * Estado:     Completo
 */

import { Component, inject, signal } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { OversightApi } from '../../core/api/oversight.api';
import { PageHeaderComponent } from '../../core/layout/page-header.component';
import { PagedResult } from '../../core/models/meta.models';
import { AuditEntry } from '../../core/models/oversight.models';
import { EmptyStateComponent } from '../../shared/components/empty-state.component';
import { CrDatePipe } from '../../shared/pipes/cr-date.pipe';

const ACT: Record<string, string> = { Create: 'Creó', Update: 'Modificó', Delete: 'Eliminó' };
const ENT: Record<string, string | undefined> = { Service: 'Servicio', ServiceLine: 'Componente', Invoice: 'Factura', DeathCertificate: 'Acta', Setting: 'Parámetro', Component: 'Catálogo: componente', Package: 'Catálogo: paquete', CasketModel: 'Catálogo: cofre' };

@Component({
  selector: 'app-audit-log-page',
  standalone: true,
  imports: [FormsModule, PageHeaderComponent, EmptyStateComponent, CrDatePipe],
  template: `
    <app-page-header title="Bitácora" description="Quién cambió qué y cuándo. Los registros nunca se editan ni se borran." [rf]="['RF-31']" />
    <div class="filters">
      <select [(ngModel)]="entity" (ngModelChange)="load(1)" aria-label="Entidad"><option value="">Todas las entidades</option>@for (e of entities; track e[0]) { <option [value]="e[0]">{{ e[1] }}</option> }</select>
      <input [(ngModel)]="actor" (ngModelChange)="load(1)" placeholder="Actor (rol o sede)" aria-label="Actor" /></div>
    @if (!data()?.items?.length) { <app-empty-state icon="history" text="No hay registros con estos filtros." /> } @else {
      <table class="dense"><thead><tr><th>Cuándo</th><th>Quién</th><th>Acción</th><th>Detalle</th></tr></thead>
        <tbody>@for (e of data()!.items; track e.id) { <tr><td>{{ e.at | crDate }}</td><td>{{ e.actorName }}</td><td>{{ act[e.action] }} {{ entity_(e.entity) }}</td><td class="detail">{{ summary(e) }}</td></tr> }</tbody></table>
      <div class="pager"><span class="muted">Página {{ data()!.page }} · {{ data()!.total }} registros</span>
        <button class="btn ghost" type="button" [disabled]="data()!.page <= 1" (click)="load(data()!.page - 1)" aria-label="Anterior"><span class="ms">chevron_left</span></button>
        <button class="btn ghost" type="button" [disabled]="data()!.page * data()!.pageSize >= data()!.total" (click)="load(data()!.page + 1)" aria-label="Siguiente"><span class="ms">chevron_right</span></button></div> }`,
  styles: [`.filters { display: flex; gap: 8px; margin-bottom: 12px; } .detail { color: var(--text-muted); max-width: 520px; overflow: hidden; text-overflow: ellipsis; white-space: nowrap; } .pager { display: flex; justify-content: flex-end; align-items: center; gap: 8px; padding-top: 8px; }`],
})
export class AuditLogPage {
  private api = inject(OversightApi);
  readonly act = ACT; readonly ent = ENT; readonly entities = Object.entries(ENT) as [string, string][];
  readonly data = signal<PagedResult<AuditEntry> | null>(null);
  entity = ''; actor = '';
  constructor() { this.load(1); }
  load(page: number): void { this.api.audit({ entity: this.entity || undefined, actor: this.actor || undefined, page }).subscribe(d => this.data.set(d)); }
  entity_(n: string): string { return ENT[n] ?? n; }
  summary(e: AuditEntry): string {
    try {
      const o = JSON.parse(e.oldValues) as Record<string, unknown>; const n = JSON.parse(e.newValues) as Record<string, unknown>;
      if (e.action === 'Delete') return `Se eliminó${o['componentName'] ? ' «' + o['componentName'] + '»' : ''}`;
      return Object.entries(n).filter(([k]) => k !== 'componentName').map(([k, v]) => `${k}: ${e.action === 'Update' ? String(o[k] ?? '') + ' → ' : ''}${String(v)}`).slice(0, 4).join(' · ');
    } catch { return ''; }
  }
}
