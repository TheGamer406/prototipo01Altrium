/**
 * Archivo:    frontend/src/app/features/home/home.page.ts
 * Qué hace:   Inicio: resumen de la sede.
 * Recibe:     Sede y rol actuales.
 * Procesa:    Pide overview de la sede, servicios de hoy, alertas abiertas, capillas y stock bajo; se
 *               refresca con el hub.
 * Devuelve:   KPI + lista de servicios de hoy + alertas + capillas ocupadas + cofres bajo mínimo.
 * Se conecta: → OversightApi, ServicesApi, OpsHubService
 * Reglas:     RF-19 · RF-29
 * Estado:     Completo
 */

import { Component, effect, inject, signal } from '@angular/core';
import { RouterLink } from '@angular/router';
import { OversightApi } from '../../core/api/oversight.api';
import { ServicesApi } from '../../core/api/services.api';
import { PageHeaderComponent } from '../../core/layout/page-header.component';
import { Alert, Overview } from '../../core/models/oversight.models';
import { ServiceListItem } from '../../core/models/services.models';
import { OpsHubService } from '../../core/realtime/ops-hub.service';
import { BranchContextService } from '../../core/state/branch-context.service';
import { OpenTabsService } from '../../core/state/open-tabs.service';
import { EmptyStateComponent } from '../../shared/components/empty-state.component';
import { KpiCardComponent } from '../../shared/components/kpi-card.component';
import { StatusChipComponent } from '../../shared/components/status-chip.component';
import { CrDatePipe } from '../../shared/pipes/cr-date.pipe';

@Component({
  selector: 'app-home-page',
  standalone: true,
  imports: [RouterLink, PageHeaderComponent, KpiCardComponent, StatusChipComponent, EmptyStateComponent, CrDatePipe],
  template: `
    <app-page-header [title]="'Inicio · ' + branch.branchName()" description="Resumen de la sede." [rf]="['RF-19','RF-29']"><a class="btn primary" routerLink="/servicios/nuevo"><span class="ms">add</span>Nuevo servicio</a></app-page-header>
    <div class="kpis"><app-kpi-card label="Servicios en curso" [value]="ov()?.kpis?.ongoing ?? '—'" /><app-kpi-card label="Capillas ocupadas" [value]="ov()?.kpis?.chapelsBusy ?? '—'" />
      <app-kpi-card label="Cofres bajo mínimo" [value]="ov()?.kpis?.lowStock ?? '—'" /><app-kpi-card label="Alertas abiertas" [value]="ov()?.kpis?.openAlerts ?? '—'" /></div>
    <div class="grid">
      <section class="card"><h2>Próximos servicios</h2>@if (!services().length) { <app-empty-state image="/img/otros/velorioMuestra3.webp" text="No hay servicios cercanos." /> }
        <table class="dense"><tbody>@for (s of services(); track s.id) { <tr class="clickable" (click)="tabs.open(s.id, s.code)"><td><strong>{{ s.code }}</strong></td><td>{{ s.deceasedName }}</td><td>{{ s.scheduledAt | crDate }}</td><td><app-status-chip [status]="s.status" /></td></tr> }</tbody></table></section>
      <section class="card"><h2>Alertas abiertas</h2>@if (!alerts().length) { <app-empty-state icon="notifications_off" text="Sin alertas." /> }
        <ul>@for (a of alerts(); track a.id) { <li><span>{{ a.message }}</span><small class="muted">{{ a.branchName }} · {{ a.createdAt | crDate }}</small></li> }</ul></section>
    </div>`,
  styles: [`.kpis { display: flex; gap: 12px; margin-bottom: 16px; flex-wrap: wrap; } .grid { display: grid; grid-template-columns: 1.3fr 1fr; gap: 12px; } ul { list-style: none; padding: 0; margin: 8px 0 0; }
    li { display: flex; flex-direction: column; gap: 2px; padding: 8px 0; border-bottom: 1px solid var(--border); font-size: 13px; }
    @media (max-width: 1024px) { .grid { grid-template-columns: 1fr; } }`],
})
export class HomePage {
  private oversight = inject(OversightApi);
  private api = inject(ServicesApi);
  readonly branch = inject(BranchContextService);
  readonly tabs = inject(OpenTabsService);
  readonly ov = signal<Overview | null>(null);
  readonly services = signal<ServiceListItem[]>([]);
  readonly alerts = signal<Alert[]>([]);
  constructor() {
    effect(() => { const b = this.branch.branchId(); this.load(b); });
    const hub = inject(OpsHubService);
    hub.serviceUpdated$.subscribe(() => this.load(this.branch.branchId()));
    hub.alertRaised$.subscribe(() => this.load(this.branch.branchId()));
  }
  private load(b: number | null): void {
    this.oversight.overview().subscribe(o => this.ov.set(b === null ? o : { ...o, kpis: this.sum(o, b) }));
    this.api.list({ branch: b, pageSize: 8, sort: 'scheduledAt desc', status: '' }).subscribe(r => this.services.set(r.items));
    this.oversight.alerts(b).subscribe(a => this.alerts.set(a.slice(0, 6)));
  }
  private sum(o: Overview, b: number): Overview['kpis'] {
    const x = o.branches.find(y => y.id === b);
    return { ongoing: x?.ongoing ?? 0, chapelsBusy: x?.chapelsBusy ?? 0, lowStock: x?.lowStock ?? 0, openAlerts: o.kpis.openAlerts };
  }
}
