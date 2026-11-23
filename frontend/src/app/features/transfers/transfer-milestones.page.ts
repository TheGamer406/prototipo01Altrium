/**
 * Archivo:    frontend/src/app/features/transfers/transfer-milestones.page.ts
 * Qué hace:   Hitos de un traslado.
 * Recibe:     Ruta :id.
 * Procesa:    Lista de hitos con hora registrada, hora sincronizada y origen (Online/Offline); tramos
 *               fuera de umbral.
 * Devuelve:   Línea de tiempo.
 * Se conecta: → LogisticsApi
 * Reglas:     RF-22 · RF-24
 * Estado:     Completo
 */

import { Component, inject, signal } from '@angular/core';
import { ActivatedRoute, RouterLink } from '@angular/router';
import { LogisticsApi } from '../../core/api/logistics.api';
import { BranchContextService } from '../../core/state/branch-context.service';
import { Transfer } from '../../core/models/logistics.models';
import { CrDatePipe } from '../../shared/pipes/cr-date.pipe';

const T: Record<string, string> = { Departure: 'Salida de la sede', Pickup: 'Recogida', ArrivalWake: 'Llegada a velación', DepartureWake: 'Salida de velación', ArrivalDestination: 'Llegada al destino' };

@Component({
  selector: 'app-transfer-milestones-page',
  standalone: true,
  imports: [RouterLink, CrDatePipe],
  template: `<p><a routerLink="/traslados">← Volver al mapa</a></p>
    @if (t(); as t) { <h1>{{ t.serviceCode }} <span class="muted">{{ t.plate }} · {{ t.driverName }}</span></h1>
      <table class="dense"><thead><tr><th>Hito</th><th>Registrado</th><th>Sincronizado</th><th>Origen</th><th>Tramo</th></tr></thead>
        <tbody>@for (m of t.milestones; track m.clientUuid; let i = $index) { <tr><td>{{ names[m.type] }}</td><td>{{ m.recordedAt | crDate }}</td><td>{{ m.syncedAt | crDate }}</td>
          <td>{{ m.source === 'Offline' ? 'Sin señal' : 'En línea' }}</td><td [class.late]="gap(i) > 20">{{ i ? gap(i) + ' min' : '—' }}</td></tr> }</tbody></table> }`,
  styles: [`.late { color: var(--warning); font-weight: 600; } h1 { margin-bottom: 12px; }`],
})
export class TransferMilestonesPage {
  readonly names = T;
  readonly t = signal<Transfer | null>(null);
  constructor() {
    const id = Number(inject(ActivatedRoute).snapshot.paramMap.get('id'));
    inject(LogisticsApi).transfers(inject(BranchContextService).branchId(), true).subscribe(l => this.t.set(l.find(x => x.id === id) ?? null));
  }
  gap(i: number): number { const m = this.t()!.milestones; return Math.round((new Date(m[i].recordedAt).getTime() - new Date(m[i - 1].recordedAt).getTime()) / 60000); }
}
