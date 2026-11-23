/**
 * Archivo:    frontend/src/app/features/messages/sent-messages.page.ts
 * Qué hace:   Correos simulados enviados.
 * Recibe:     Filtros tipo/servicio.
 * Procesa:    Lista + vista del correo.
 * Devuelve:   Tabla + panel de lectura.
 * Se conecta: → OversightApi
 * Reglas:     RF-12 · RF-35
 * Estado:     Completo
 */

import { Component, inject, signal } from '@angular/core';
import { OversightApi } from '../../core/api/oversight.api';
import { PageHeaderComponent } from '../../core/layout/page-header.component';
import { PagedResult } from '../../core/models/meta.models';
import { Message } from '../../core/models/oversight.models';
import { CrDatePipe } from '../../shared/pipes/cr-date.pipe';

const K: Record<string, string> = { FamilyStatus: 'Estado a la familia', FamilyInvoice: 'Factura a la familia', InsurerNotice: 'Aviso a aseguradora', ProviderOffer: 'Oferta a proveedor', Alert: 'Alerta' };

@Component({
  selector: 'app-sent-messages-page',
  standalone: true,
  imports: [PageHeaderComponent, CrDatePipe],
  template: `
    <app-page-header title="Mensajes" description="Correos simulados que envió el sistema (la familia no tiene usuario)." [rf]="['RF-12','RF-15','RF-35']" />
    <select (change)="load(1, $any($event.target).value)" aria-label="Tipo"><option value="">Todos los tipos</option>@for (k of kinds; track k[0]) { <option [value]="k[0]">{{ k[1] }}</option> }</select>
    <table class="dense"><thead><tr><th>Fecha</th><th>Tipo</th><th>Para</th><th>Asunto</th><th>Servicio</th></tr></thead>
      <tbody>@for (m of data()?.items ?? []; track m.id) { <tr [title]="m.body"><td>{{ m.createdAt | crDate }}</td><td>{{ names[m.kind] }}</td><td>{{ m.toAddress }}</td><td>{{ m.subject }}</td><td>{{ m.serviceCode ?? '—' }}</td></tr> }</tbody></table>
    @if (data(); as d) { <div class="pager"><span class="muted">Página {{ d.page }} · {{ d.total }} correos</span>
      <button class="btn ghost" type="button" [disabled]="d.page <= 1" (click)="load(d.page - 1, kind)" aria-label="Anterior"><span class="ms">chevron_left</span></button>
      <button class="btn ghost" type="button" [disabled]="d.page * d.pageSize >= d.total" (click)="load(d.page + 1, kind)" aria-label="Siguiente"><span class="ms">chevron_right</span></button></div> }`,
  styles: [`select { margin-bottom: 12px; } .pager { display: flex; justify-content: flex-end; align-items: center; gap: 8px; padding-top: 8px; }`],
})
export class SentMessagesPage {
  private api = inject(OversightApi);
  readonly names = K; readonly kinds = Object.entries(K);
  readonly data = signal<PagedResult<Message> | null>(null);
  kind = '';
  constructor() { this.load(1, ''); }
  load(page: number, kind: string): void { this.kind = kind; this.api.messages({ page, kind: kind || undefined }).subscribe(d => this.data.set(d)); }
}
