/**
 * Archivo:    frontend/src/app/core/layout/topbar.component.ts
 * Qué hace:   Barra superior: logo, migas, búsqueda global, sede, rol, alertas, modo admin.
 * Recibe:     Estado de los servicios de contexto + alertas del hub.
 * Procesa:    Búsqueda con debounce (300 ms) a GET /api/services?q=; abre el servicio como pestaña
 *               ERP.
 * Devuelve:   Cambios de sede/rol y navegación.
 * Se conecta: ← shell · → branch/role/admin-mode services, ServicesApi, AlertsApi
 * Reglas:     RF-29 · RF-30
 * Estado:     Completo
 */

import { Component, DestroyRef, computed, inject, signal } from '@angular/core';
import { takeUntilDestroyed, toObservable } from '@angular/core/rxjs-interop';
import { Router, NavigationEnd } from '@angular/router';
import { FormsModule } from '@angular/forms';
import { debounceTime, distinctUntilChanged, filter, of, switchMap } from 'rxjs';
import { MatMenuModule } from '@angular/material/menu';
import { ServicesApi } from '../api/services.api';
import { OversightApi } from '../api/oversight.api';
import { MetaApi } from '../api/meta.api';
import { ServiceListItem } from '../models/services.models';
import { Alert } from '../models/oversight.models';
import { ProviderMeta } from '../models/meta.models';
import { AdminModeService } from '../state/admin-mode.service';
import { BranchContextService } from '../state/branch-context.service';
import { LayoutService } from '../state/layout.service';
import { OpenTabsService } from '../state/open-tabs.service';
import { RoleContextService } from '../state/role-context.service';
import { OpsHubService } from '../realtime/ops-hub.service';
import { NAV } from '../navigation/nav-items';
import { LogoComponent } from '../../shared/components/logo.component';
import { StatusChipComponent } from '../../shared/components/status-chip.component';
import { CrDatePipe } from '../../shared/pipes/cr-date.pipe';
import { AdminMenuComponent } from './admin-menu.component';

@Component({
  selector: 'app-topbar',
  standalone: true,
  imports: [FormsModule, MatMenuModule, LogoComponent, StatusChipComponent, CrDatePipe, AdminMenuComponent],
  template: `
    <div class="bar">
      <button class="btn ghost menu" type="button" (click)="layout.toggleMobile()" aria-label="Abrir menú"><span class="ms">menu</span></button>
      <app-logo [compact]="layout.isMobile()" />
      <span class="crumb muted">{{ crumb() }}</span>

      <div class="search">
        <span class="ms">search</span>
        <input type="search" placeholder="Buscar servicio, fallecido o cédula" aria-label="Búsqueda global" [ngModel]="query()" (ngModelChange)="query.set($event)" (blur)="closeResults()" />
        @if (results().length) {
          <ul class="results" role="listbox">
            @for (r of results(); track r.id) {
              <li role="option" (mousedown)="openResult(r)">
                <strong>{{ r.code }}</strong> <span>{{ r.deceasedName }}</span><span class="muted">{{ r.branchName }}</span><app-status-chip [status]="r.status" />
              </li>
            }
          </ul>
        }
      </div>

      <span class="spacer"></span>

      <label class="sel"><span class="muted">Sede</span>
        <select [ngModel]="branch.branchId()" (ngModelChange)="branch.set($event)" [disabled]="role.role() !== 'Management' && false" aria-label="Sede actual">
          @if (role.role() === 'Management') { <option [ngValue]="null">Todas</option> }
          @for (b of branch.branches(); track b.id) { <option [ngValue]="b.id">{{ b.name }}</option> }
        </select>
      </label>
      <label class="sel"><span class="muted">Rol</span>
        <select [ngModel]="role.role()" (ngModelChange)="changeRole($event)" aria-label="Rol actual">
          @for (r of role.roles; track r.value) { <option [value]="r.value">{{ r.label }}</option> }
        </select>
      </label>
      @if (role.role() === 'Provider') {
        <label class="sel"><span class="muted">Proveedor</span>
          <select [ngModel]="role.providerId()" (ngModelChange)="role.setProvider($event)" aria-label="Proveedor actual">
            @for (p of providers(); track p.id) { <option [ngValue]="p.id">{{ p.name }}</option> }
          </select>
        </label>
      }

      <button class="btn ghost bell" type="button" [matMenuTriggerFor]="alertsMenu" aria-label="Alertas">
        <span class="ms">notifications</span>
        @if (alerts().length) { <span class="badge">{{ alerts().length }}</span> }
      </button>
      <mat-menu #alertsMenu="matMenu" class="alerts-panel">
        @if (!alerts().length) { <div class="none muted">Sin alertas abiertas.</div> }
        @for (a of alerts(); track a.id) {
          <div class="alert" (click)="$event.stopPropagation()">
            <div class="msg">{{ a.message }}</div>
            <div class="meta muted">{{ a.branchName }} · {{ a.createdAt | crDate:'datetime' }}</div>
            <button class="btn ghost" type="button" (click)="ack(a)">Marcar como vista</button>
          </div>
        }
      </mat-menu>

      <label class="toggle" title="Modo admin del demo">
        <input type="checkbox" [checked]="admin.enabled()" (change)="admin.toggle()" /> <span>Admin</span>
      </label>
      @if (admin.enabled()) { <app-admin-menu /> }
    </div>`,
  styles: [`
    .bar { display: flex; align-items: center; gap: 12px; height: var(--topbar-h); padding: 0 16px; }
    .crumb { margin-left: 8px; } .spacer { flex: 1; }
    .search { position: relative; display: flex; align-items: center; gap: 6px; margin-left: 12px; }
    .search .ms { position: absolute; left: 8px; color: var(--text-muted); font-size: 18px; } .search input { padding-left: 32px; width: 300px; }
    .results { position: absolute; top: 38px; left: 0; width: 460px; margin: 0; padding: 4px; list-style: none; background: var(--surface-2);
      border: 1px solid var(--border); border-radius: var(--radius); box-shadow: 0 8px 24px rgba(0,0,0,.45); z-index: 20; }
    .results li { display: flex; align-items: center; gap: 10px; height: 36px; padding: 0 8px; border-radius: 4px; cursor: pointer; font-size: 13px; }
    .results li:hover { background: var(--accent-soft); } .results li .muted { margin-left: auto; }
    .sel { display: flex; align-items: center; gap: 6px; font-size: 12px; } .sel select { max-width: 190px; }
    .bell { position: relative; padding: 0 8px; } .badge { position: absolute; top: 0; right: 0; min-width: 16px; height: 16px; border-radius: 8px;
      background: var(--danger); color: #fff; font-size: 10px; display: grid; place-items: center; }
    .toggle { display: flex; align-items: center; gap: 6px; font-size: 12px; cursor: pointer; } .toggle input { height: auto; }
    .alert { padding: 8px 14px; border-bottom: 1px solid var(--border); max-width: 380px; } .alert .msg { font-size: 13px; } .alert .meta { font-size: 11px; margin: 2px 0 4px; }
    .none { padding: 12px 16px; } .menu { display: none; padding: 0 6px; }
    @media (max-width: 1024px) { .crumb { display: none; } .search input { width: 200px; } .sel .muted { display: none; } .bar { gap: 8px; padding: 0 10px; } }
    @media (max-width: 768px) { .menu { display: inline-flex; } .search, .toggle span { display: none; } .sel select { max-width: 110px; } .results { width: 90vw; } }`],
})
export class TopbarComponent {
  private router = inject(Router);
  private servicesApi = inject(ServicesApi);
  private oversight = inject(OversightApi);
  private meta = inject(MetaApi);
  private tabs = inject(OpenTabsService);
  private hub = inject(OpsHubService);
  private destroyRef = inject(DestroyRef);
  readonly role = inject(RoleContextService);
  readonly branch = inject(BranchContextService);
  readonly admin = inject(AdminModeService);
  readonly layout = inject(LayoutService);

  readonly query = signal('');
  readonly results = signal<ServiceListItem[]>([]);
  readonly alerts = signal<Alert[]>([]);
  readonly providers = signal<ProviderMeta[]>([]);
  private url = signal(this.router.url);
  readonly crumb = computed(() => {
    const seg = '/' + (this.url().split('?')[0].split('/')[1] ?? '');
    const item = NAV.flatMap(s => s.items).find(i => i.route === seg);
    return item ? `${NAV.find(s => s.items.includes(item))!.title} / ${item.label}` : '';
  });

  constructor() {
    this.router.events.pipe(filter(e => e instanceof NavigationEnd), takeUntilDestroyed()).subscribe(() => this.url.set(this.router.url));
    toObservable(this.query).pipe(
      debounceTime(300), distinctUntilChanged(),
      switchMap(q => (q.trim().length >= 2 ? this.servicesApi.list({ q: q.trim(), pageSize: 6 }) : of(null))),
      takeUntilDestroyed(),
    ).subscribe(r => this.results.set(r?.items ?? []));

    this.meta.providers().subscribe(list => {
      this.providers.set(list);
      if (this.role.providerId() === null && list.length) this.role.setProvider(list.find(p => p.type === 'Flowers')?.id ?? list[0].id);
    });
    this.loadAlerts();
    toObservable(computed(() => `${this.role.role()}-${this.branch.branchId()}`)).pipe(takeUntilDestroyed()).subscribe(() => this.loadAlerts());
    this.hub.alertRaised$.pipe(takeUntilDestroyed(this.destroyRef)).subscribe(a => this.alerts.update(list => [a, ...list.filter(x => x.id !== a.id)]));
  }

  private loadAlerts(): void { this.oversight.alerts(this.branch.branchId()).subscribe({ next: a => this.alerts.set(a), error: () => undefined }); }

  changeRole(value: string): void {
    this.role.setRole(value as never);
    this.branch.ensureFor(value);
    if (value === 'Management') this.branch.set(null);
  }

  openResult(r: ServiceListItem): void { this.query.set(''); this.results.set([]); this.tabs.open(r.id, r.code); }
  closeResults(): void { setTimeout(() => this.results.set([]), 150); }
  ack(a: Alert): void { this.oversight.ack(a.id).subscribe(() => this.alerts.update(l => l.filter(x => x.id !== a.id))); }
}
