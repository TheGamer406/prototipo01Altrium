/**
 * Archivo:    frontend/src/app/features/inventory/inventory-alerts.page.ts
 * Qué hace:   Alertas de stock bajo.
 * Recibe:     Sede.
 * Procesa:    AlertsApi tipo LowStock.
 * Devuelve:   Lista.
 * Se conecta: → OversightApi
 * Reglas:     RF-27
 * Estado:     Completo
 */

import { Component, effect, inject, signal } from '@angular/core';
import { OversightApi } from '../../core/api/oversight.api';
import { Alert } from '../../core/models/oversight.models';
import { BranchContextService } from '../../core/state/branch-context.service';
import { OpsHubService } from '../../core/realtime/ops-hub.service';
import { EmptyStateComponent } from '../../shared/components/empty-state.component';
import { CrDatePipe } from '../../shared/pipes/cr-date.pipe';

@Component({
  selector: 'app-inventory-alerts-page',
  standalone: true,
  imports: [EmptyStateComponent, CrDatePipe],
  template: `@if (!items().length) { <app-empty-state icon="inventory" text="Sin alertas de inventario abiertas." /> }
    <table class="dense">@for (a of items(); track a.id) { <tr><td>{{ a.message }}</td><td class="muted">{{ a.branchName }}</td><td class="muted">{{ a.createdAt | crDate }}</td><td><button class="btn ghost" type="button" (click)="ack(a)">Marcar vista</button></td></tr> }</table>`,
})
export class InventoryAlertsPage {
  private api = inject(OversightApi);
  private branch = inject(BranchContextService);
  readonly items = signal<Alert[]>([]);
  constructor() {
    effect(() => { this.branch.branchId(); this.load(); });
    inject(OpsHubService).alertRaised$.subscribe(() => this.load());
  }
  private load(): void { this.api.alerts(this.branch.branchId()).subscribe(a => this.items.set(a.filter(x => x.type === 'LowStock'))); }
  ack(a: Alert): void { this.api.ack(a.id).subscribe(() => this.load()); }
}
