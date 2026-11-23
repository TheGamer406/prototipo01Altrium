/**
 * Archivo:    frontend/src/app/features/provider-inbox/provider-inbox.page.ts
 * Qué hace:   Bandeja del proveedor (rol Proveedor).
 * Recibe:     Proveedor elegido (selector en la página para el demo).
 * Procesa:    Ofertas abiertas con lugar, hora y precio fijo; 'Aceptar' con un clic; historial; nuevas
 *               ofertas llegan por el hub.
 * Devuelve:   Resultado 'Adjudicado' o 'Ya fue asignada'.
 * Se conecta: → ProvidersApi, OpsHubService
 * Reglas:     RF-15 · RF-16
 * Estado:     Completo
 */

import { Component, effect, inject, signal } from '@angular/core';
import { MatSnackBar } from '@angular/material/snack-bar';
import { ProvidersApi } from '../../core/api/providers.api';
import { Offer } from '../../core/models/providers.models';
import { OpsHubService } from '../../core/realtime/ops-hub.service';
import { RoleContextService } from '../../core/state/role-context.service';
import { PageHeaderComponent } from '../../core/layout/page-header.component';
import { EmptyStateComponent } from '../../shared/components/empty-state.component';
import { StatusChipComponent } from '../../shared/components/status-chip.component';
import { ColonesPipe } from '../../shared/pipes/colones.pipe';
import { CrDatePipe } from '../../shared/pipes/cr-date.pipe';

@Component({
  selector: 'app-provider-inbox-page',
  standalone: true,
  imports: [PageHeaderComponent, EmptyStateComponent, StatusChipComponent, ColonesPipe, CrDatePipe],
  template: `
    <app-page-header title="Bandeja del proveedor" description="Acepte con un clic: gana el primero que responda." [rf]="['RF-15','RF-16']" />
    <h2>Ofertas abiertas</h2>
    @if (!open().length) { <app-empty-state image="/img/otros/velorioMuestra2.webp" text="No hay ofertas abiertas por ahora." /> }
    <div class="grid">@for (o of open(); track o.id) {
      <article class="card"><header><strong>{{ o.componentName }}</strong><app-status-chip [status]="o.status" kind="offer" /></header>
        @if (o.imagePath) { <img [src]="'/' + o.imagePath" alt="" /> }
        <p class="muted">{{ o.serviceCode }} · {{ o.place }}</p><p>{{ o.scheduledAt | crDate }} · precio fijo <strong>{{ o.fixedPriceCrc | colones }}</strong></p>
        <p class="muted">Responder antes de {{ o.expiresAt | crDate }}</p>
        <button class="btn primary" type="button" [disabled]="busy() === o.id" (click)="accept(o)">Aceptar</button></article> }</div>
    <h2>Historial</h2>
    <table class="dense"><thead><tr><th>Servicio</th><th>Componente</th><th>Fecha</th><th class="num">Precio</th><th>Resultado</th></tr></thead>
      <tbody>@for (o of history(); track o.id) { <tr><td>{{ o.serviceCode }}</td><td>{{ o.componentName }}</td><td>{{ o.scheduledAt | crDate }}</td><td class="num">{{ o.fixedPriceCrc | colones }}</td><td><app-status-chip [status]="o.status" kind="offer" /></td></tr> }</tbody></table>`,
  styles: [`.grid { display: grid; grid-template-columns: repeat(auto-fill, minmax(260px, 1fr)); gap: 12px; margin: 8px 0 20px; } header { display: flex; justify-content: space-between; } p { margin: 6px 0; }
    img { width: 100%; height: 90px; object-fit: cover; border-radius: var(--radius); margin-top: 8px; } h2 { margin: 12px 0 8px; }`],
})
export class ProviderInboxPage {
  private api = inject(ProvidersApi);
  private role = inject(RoleContextService);
  private snack = inject(MatSnackBar);
  readonly open = signal<Offer[]>([]);
  readonly history = signal<Offer[]>([]);
  readonly busy = signal<string | null>(null);
  constructor() {
    effect(() => { const p = this.role.providerId(); if (p) this.load(p); });
    const hub = inject(OpsHubService);
    hub.offerAwarded$.subscribe(() => this.reload());
    hub.serviceUpdated$.subscribe(() => this.reload());
  }
  private reload(): void { const p = this.role.providerId(); if (p) this.load(p); }
  private load(p: number): void { this.api.inbox(p).subscribe(i => { this.open.set(i.open); this.history.set(i.history); }); }
  accept(o: Offer): void {
    this.busy.set(o.id);
    this.api.accept(o.id, this.role.providerId() ?? undefined).subscribe({
      next: r => { this.busy.set(null); this.snack.open(r.won ? '¡Adjudicado! El servicio es suyo.' : 'Ya fue asignada a otro proveedor.', 'Cerrar', { duration: 5000, panelClass: r.won ? 'success' : 'error' }); this.reload(); },
      error: () => { this.busy.set(null); this.reload(); },
    });
  }
}
