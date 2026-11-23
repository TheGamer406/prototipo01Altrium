/**
 * Archivo:    frontend/src/app/features/providers/open-opportunities.page.ts
 * Qué hace:   Componentes aún sin proveedor.
 * Recibe:     Sede.
 * Procesa:    Lista con minutos abiertos; vencidas resaltadas; se refresca con el hub.
 * Devuelve:   Tabla.
 * Se conecta: → ProvidersApi, OpsHubService
 * Reglas:     RF-17
 * Estado:     Completo
 */

import { Component, effect, inject, signal } from '@angular/core';
import { ProvidersApi } from '../../core/api/providers.api';
import { OpenOpportunity } from '../../core/models/providers.models';
import { BranchContextService } from '../../core/state/branch-context.service';
import { OpsHubService } from '../../core/realtime/ops-hub.service';
import { EmptyStateComponent } from '../../shared/components/empty-state.component';

@Component({
  selector: 'app-open-opportunities-page',
  standalone: true,
  imports: [EmptyStateComponent],
  template: `@if (!items().length) { <app-empty-state icon="task_alt" text="No hay componentes esperando proveedor." /> } @else {
    <table class="dense"><thead><tr><th>Servicio</th><th>Sede</th><th>Componente</th><th>Ofertas enviadas</th><th>Minutos abierta</th><th></th></tr></thead>
      <tbody>@for (o of items(); track o.lineId) { <tr><td><strong>{{ o.serviceCode }}</strong></td><td>{{ o.branchName }}</td><td>{{ o.componentName }}</td><td>{{ o.offersSent }}</td><td>{{ o.minutesOpen }}</td>
        <td>@if (o.isOverdue) { <span class="late">● Vencida: asignar manualmente</span> }</td></tr> }</tbody></table> }`,
  styles: [`.late { color: var(--danger); font-size: 12px; }`],
})
export class OpenOpportunitiesPage {
  private api = inject(ProvidersApi);
  private branch = inject(BranchContextService);
  readonly items = signal<OpenOpportunity[]>([]);
  constructor() {
    effect(() => { const b = this.branch.branchId(); this.load(b); });
    const hub = inject(OpsHubService);
    hub.offerAwarded$.subscribe(() => this.load(this.branch.branchId()));
    hub.alertRaised$.subscribe(() => this.load(this.branch.branchId()));
  }
  private load(b: number | null): void { this.api.opportunities(b).subscribe(r => this.items.set(r)); }
}
