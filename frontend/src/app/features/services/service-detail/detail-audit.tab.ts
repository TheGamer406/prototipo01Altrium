/**
 * Archivo:    frontend/src/app/features/services/service-detail/detail-audit.tab.ts
 * Qué hace:   Bitácora del servicio.
 * Recibe:     serviceId.
 * Procesa:    AuditQuery filtrado por servicio; muestra valor anterior y nuevo.
 * Devuelve:   Vista de la pestaña; acciones vía el Api correspondiente.
 * Se conecta: ← service-detail.page
 * Reglas:     RF-31
 * Estado:     Completo
 */

import { Component, Input, OnChanges, inject, signal } from '@angular/core';
import { ServicesApi } from '../../../core/api/services.api';
import { TimelineEntry } from '../../../core/models/services.models';
import { CrDatePipe } from '../../../shared/pipes/cr-date.pipe';

const ICON = { status: 'swap_horiz', audit: 'edit_note', message: 'mail' } as const;

@Component({
  selector: 'app-detail-audit-tab',
  standalone: true,
  imports: [CrDatePipe],
  template: `<div class="card"><h2>Línea de tiempo</h2><ol>@for (e of items(); track $index) { <li><span class="ms">{{ icon(e.kind) }}</span><span class="when muted">{{ e.at | crDate }}</span><span>{{ e.text }}</span></li> }</ol></div>`,
  styles: [`ol { list-style: none; padding: 0; margin: 8px 0 0; } li { display: flex; gap: 12px; align-items: center; height: var(--row-h); border-bottom: 1px solid var(--border); font-size: 13px; } .when { width: 130px; } .ms { color: var(--accent-text); font-size: 18px; }`],
})
export class DetailAuditTab implements OnChanges {
  private api = inject(ServicesApi);
  @Input({ required: true }) serviceId!: string;
  readonly items = signal<TimelineEntry[]>([]);
  icon(k: TimelineEntry['kind']): string { return ICON[k]; }
  ngOnChanges(): void { this.api.timeline(this.serviceId).subscribe(t => this.items.set(t)); }
}
