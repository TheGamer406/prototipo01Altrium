/**
 * Archivo:    frontend/src/app/features/services/service-detail/detail-messages.tab.ts
 * Qué hace:   Correos enviados del servicio.
 * Recibe:     serviceId.
 * Procesa:    MessagesApi filtrado.
 * Devuelve:   Vista de la pestaña; acciones vía el Api correspondiente.
 * Se conecta: ← service-detail.page
 * Reglas:     RF-12 · RF-35
 * Estado:     Completo
 */

import { Component, Input, OnChanges, inject, signal } from '@angular/core';
import { OversightApi } from '../../../core/api/oversight.api';
import { Message } from '../../../core/models/oversight.models';
import { EmptyStateComponent } from '../../../shared/components/empty-state.component';
import { CrDatePipe } from '../../../shared/pipes/cr-date.pipe';

@Component({
  selector: 'app-detail-messages-tab',
  standalone: true,
  imports: [CrDatePipe, EmptyStateComponent],
  template: `<div class="card">@if (!items().length) { <app-empty-state icon="mail" text="Aún no se han enviado correos de este servicio." /> }
    @for (m of items(); track m.id) { <article><header><strong>{{ m.subject }}</strong><span class="muted">{{ m.createdAt | crDate }} · para {{ m.toAddress }}</span></header><p>{{ m.body }}</p></article> }</div>`,
  styles: [`article { border-bottom: 1px solid var(--border); padding: 8px 0; } header { display: flex; justify-content: space-between; gap: 12px; } p { margin: 4px 0 0; color: var(--text-muted); }`],
})
export class DetailMessagesTab implements OnChanges {
  private api = inject(OversightApi);
  @Input({ required: true }) serviceId!: string;
  readonly items = signal<Message[]>([]);
  ngOnChanges(): void { this.api.messages({ serviceId: this.serviceId }).subscribe(r => this.items.set(r.items)); }
}
