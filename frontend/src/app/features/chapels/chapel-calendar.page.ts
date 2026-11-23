/**
 * Archivo:    frontend/src/app/features/chapels/chapel-calendar.page.ts
 * Qué hace:   Calendario de capillas (día y semana).
 * Recibe:     Sede, rango.
 * Procesa:    Bloques por capilla; reservar con clic; choque muestra el mensaje del backend.
 * Devuelve:   Calendario.
 * Se conecta: → LogisticsApi
 * Reglas:     RF-20
 * Estado:     Completo
 */

import { Component, effect, inject, signal } from '@angular/core';
import { LogisticsApi } from '../../core/api/logistics.api';
import { PageHeaderComponent } from '../../core/layout/page-header.component';
import { ChapelCalendar } from '../../core/models/logistics.models';
import { BranchContextService } from '../../core/state/branch-context.service';
import { EmptyStateComponent } from '../../shared/components/empty-state.component';
import { CrDatePipe } from '../../shared/pipes/cr-date.pipe';

@Component({
  selector: 'app-chapel-calendar-page',
  standalone: true,
  imports: [PageHeaderComponent, EmptyStateComponent, CrDatePipe],
  template: `
    <app-page-header title="Capillas" description="Reservas de las capillas de la sede (cada velación ocupa 24 h)." [rf]="['RF-20']" />
    <div class="filters"><label>Desde <input type="date" [value]="day()" (change)="day.set($any($event.target).value)" /></label><span class="muted">Se muestran 7 días</span></div>
    @if (!branch.branchId()) { <app-empty-state icon="map" text="Elija una sede en la barra superior." /> }
    @else {
      @for (c of cal()?.chapels ?? []; track c.id) {
        <section class="card"><header><h2>{{ c.name }}</h2><span class="muted">aforo {{ c.capacity }}</span></header>
          @if (!c.bookings.length) { <p class="free">Libre toda la semana.</p> }
          <table class="dense">@for (b of c.bookings; track b.id) { <tr><td><strong>{{ b.serviceCode }}</strong></td><td>{{ b.from | crDate }}</td><td>{{ b.to | crDate }}</td></tr> }</table>
        </section>
      } @empty { <app-empty-state image="/img/otros/velorioMuestra1.webp" text="Esta sede no tiene capillas." /> }
    }`,
  styles: [`.filters { display: flex; gap: 12px; align-items: center; margin-bottom: 12px; } section { margin-bottom: 12px; } header { display: flex; gap: 12px; align-items: baseline; } .free { color: var(--success); margin: 8px 0 0; }`],
})
export class ChapelCalendarPage {
  private api = inject(LogisticsApi);
  readonly branch = inject(BranchContextService);
  readonly day = signal(new Date().toISOString().slice(0, 10));
  readonly cal = signal<ChapelCalendar | null>(null);
  constructor() {
    effect(() => {
      const b = this.branch.branchId(); const d = this.day();
      if (b === null) { this.cal.set(null); return; }
      const from = new Date(`${d}T00:00:00-06:00`);
      this.api.chapelCalendar(b, from.toISOString(), new Date(from.getTime() + 7 * 86400000).toISOString()).subscribe(c => this.cal.set(c));
    });
  }
}
