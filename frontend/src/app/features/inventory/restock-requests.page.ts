/**
 * Archivo:    frontend/src/app/features/inventory/restock-requests.page.ts
 * Qué hace:   Solicitudes a bodega central.
 * Recibe:     Estado.
 * Procesa:    Despachar solicitud.
 * Devuelve:   Tabla.
 * Se conecta: → InventoryApi
 * Reglas:     RF-28
 * Estado:     Completo
 */

import { Component, effect, inject, signal } from '@angular/core';
import { InventoryApi } from '../../core/api/inventory.api';
import { RestockRequest } from '../../core/models/inventory.models';
import { BranchContextService } from '../../core/state/branch-context.service';
import { RoleContextService } from '../../core/state/role-context.service';
import { CrDatePipe } from '../../shared/pipes/cr-date.pipe';

const L: Record<string, string> = { Requested: 'Solicitado', Dispatched: 'Despachado', Received: 'Recibido' };

@Component({
  selector: 'app-restock-requests-page',
  standalone: true,
  imports: [CrDatePipe],
  template: `<table class="dense"><thead><tr><th>Solicitud</th><th>Sede</th><th>Modelo</th><th class="num">Cant.</th><th>Fecha</th><th>Estado</th><th></th></tr></thead>
    <tbody>@for (r of items(); track r.id) { <tr><td>{{ r.externalNumber ?? '—' }}</td><td>{{ r.branchName }}</td><td>{{ r.sku }}</td><td class="num">{{ r.qty }}</td><td>{{ r.requestedAt | crDate }}</td><td>{{ label(r.status) }}</td>
      <td>@if (r.status !== 'Received') { <button class="btn" type="button" (click)="advance(r)">{{ r.status === 'Requested' ? 'Despachar' : 'Marcar recibido' }}</button> }</td></tr> }</tbody></table>`,
})
export class RestockRequestsPage {
  private api = inject(InventoryApi);
  private branch = inject(BranchContextService);
  readonly role = inject(RoleContextService);
  readonly items = signal<RestockRequest[]>([]);
  constructor() { effect(() => { this.branch.branchId(); this.load(); }); }
  label(s: string): string { return L[s] ?? s; }
  private load(): void { this.api.restockRequests({ branch: this.branch.branchId() }).subscribe(r => this.items.set(r)); }
  advance(r: RestockRequest): void { this.api.dispatch(r.id).subscribe(() => this.load()); }
}
