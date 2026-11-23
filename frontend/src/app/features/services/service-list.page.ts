/**
 * Archivo:    frontend/src/app/features/services/service-list.page.ts
 * Qué hace:   Lista de servicios con filtros.
 * Recibe:     Query params status, branch, q, page.
 * Procesa:    Pide ServicesApi.list; filtros en la URL; clic abre pestaña ERP.
 * Devuelve:   Tabla paginada con chips de estado y marca de urgente.
 * Se conecta: → ServicesApi, OpenTabsService, paged-table
 * Reglas:     RF-19 · RF-30
 * Estado:     Completo
 */

import { Component, computed, effect, inject, signal } from '@angular/core';
import { ActivatedRoute, Router, RouterLink } from '@angular/router';
import { toSignal } from '@angular/core/rxjs-interop';
import { map } from 'rxjs';
import { ServicesApi } from '../../core/api/services.api';
import { PageHeaderComponent } from '../../core/layout/page-header.component';
import { PagedResult } from '../../core/models/meta.models';
import { ServiceListItem } from '../../core/models/services.models';
import { BranchContextService } from '../../core/state/branch-context.service';
import { OpenTabsService } from '../../core/state/open-tabs.service';
import { OpsHubService } from '../../core/realtime/ops-hub.service';
import { EmptyStateComponent } from '../../shared/components/empty-state.component';
import { SkeletonComponent } from '../../shared/components/skeleton.component';
import { StatusChipComponent } from '../../shared/components/status-chip.component';
import { ColonesPipe } from '../../shared/pipes/colones.pipe';
import { CrDatePipe } from '../../shared/pipes/cr-date.pipe';

const STATUSES = [['', 'Todos'], ['Registered', 'Registrado'], ['Invoicing', 'En facturación'], ['Paid', 'Pagado'], ['InProgress', 'En curso'], ['Completed', 'Finalizado'], ['Advance', 'Anticipado']];

@Component({
  selector: 'app-service-list-page',
  standalone: true,
  imports: [RouterLink, PageHeaderComponent, StatusChipComponent, ColonesPipe, CrDatePipe, SkeletonComponent, EmptyStateComponent],
  template: `
    <app-page-header title="Servicios" description="Todos los servicios de la sede. Clic en una fila para abrirlo en una pestaña." [rf]="['RF-19','RF-30']">
      <a class="btn primary" routerLink="/servicios/nuevo"><span class="ms">add</span>Nuevo servicio</a>
    </app-page-header>
    <div class="filters">
      <input type="search" placeholder="Código, fallecido o cédula" [value]="q()" (input)="setQ($any($event.target).value)" aria-label="Buscar" />
      <select [value]="status()" (change)="set({ status: $any($event.target).value, page: 1 })" aria-label="Estado">@for (s of statuses; track s[0]) { <option [value]="s[0]">{{ s[1] }}</option> }</select>
    </div>
    @if (loading()) { <app-skeleton [rows]="10" /> }
    @else if (!data()?.items?.length) { <app-empty-state image="/img/otros/velorioMuestra1.webp" text="No hay servicios con estos filtros." /> }
    @else {
      <table class="dense"><thead><tr><th>Código</th><th>Fallecido</th><th>Sede</th><th>Fecha</th><th>Estado</th><th class="num">Total</th></tr></thead>
        <tbody>@for (s of data()!.items; track s.id) {
          <tr class="clickable" tabindex="0" (click)="open(s)" (keydown.enter)="open(s)">
            <td><strong>{{ s.code }}</strong> @if (s.hasUrgentLine) { <span class="urgent" title="Componente por vencer en menos de 2 h">● urgente</span> } @if (s.isCustomPackage) { <span class="muted" title="Paquete personalizado">·personalizado</span> }</td>
            <td>{{ s.deceasedName }}</td><td>{{ s.branchName }}</td><td>{{ s.scheduledAt | crDate }}</td>
            <td><app-status-chip [status]="s.status" /></td><td class="num">{{ s.totalCrc | colones }}</td></tr> }</tbody></table>
      <div class="pager"><span class="muted">Página {{ data()!.page }} · {{ data()!.total }} servicios</span>
        <button class="btn ghost" type="button" [disabled]="data()!.page <= 1" (click)="set({ page: data()!.page - 1 })" aria-label="Anterior"><span class="ms">chevron_left</span></button>
        <button class="btn ghost" type="button" [disabled]="data()!.page * data()!.pageSize >= data()!.total" (click)="set({ page: data()!.page + 1 })" aria-label="Siguiente"><span class="ms">chevron_right</span></button></div>
    }`,
  styles: [`.filters { display: flex; gap: 8px; margin-bottom: 12px; } .filters input { width: 320px; max-width: 100%; } .filters { flex-wrap: wrap; } .urgent { color: var(--warning); font-size: 12px; margin-left: 6px; } .pager { display: flex; justify-content: flex-end; align-items: center; gap: 8px; padding-top: 8px; }`],
})
export class ServiceListPage {
  private api = inject(ServicesApi);
  private route = inject(ActivatedRoute);
  private router = inject(Router);
  private tabs = inject(OpenTabsService);
  private branch = inject(BranchContextService);
  private hub = inject(OpsHubService);
  readonly statuses = STATUSES;
  private params = toSignal(this.route.queryParamMap.pipe(map(p => ({ status: p.get('status') ?? '', q: p.get('q') ?? '', page: Number(p.get('page') ?? 1) }))), { initialValue: { status: '', q: '', page: 1 } });
  readonly status = computed(() => this.params().status);
  readonly q = computed(() => this.params().q);
  readonly data = signal<PagedResult<ServiceListItem> | null>(null);
  readonly loading = signal(true);
  private timer?: ReturnType<typeof setTimeout>;

  constructor() {
    effect(() => { const p = this.params(); const b = this.branch.branchId(); this.load(p, b); });
    this.hub.serviceUpdated$.subscribe(() => this.load(this.params(), this.branch.branchId(), true));
  }

  private load(p: { status: string; q: string; page: number }, branch: number | null, silent = false): void {
    if (!silent) this.loading.set(true);
    this.api.list({ status: p.status, q: p.q, page: p.page, branch, pageSize: 25 }).subscribe({ next: d => { this.data.set(d); this.loading.set(false); }, error: () => this.loading.set(false) });
  }

  set(patch: Record<string, string | number | null>): void { void this.router.navigate([], { queryParams: patch, queryParamsHandling: 'merge' }); }
  setQ(v: string): void { clearTimeout(this.timer); this.timer = setTimeout(() => this.set({ q: v || null, page: 1 }), 300); }
  open(s: ServiceListItem): void { this.tabs.open(s.id, s.code); }
}
