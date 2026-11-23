/**
 * Archivo:    frontend/src/app/features/central/ongoing-services.page.ts
 * Qué hace:   Servicios en curso en todas las sedes.
 * Recibe:     —
 * Procesa:    ServicesApi status=InProgress.
 * Devuelve:   Tabla.
 * Se conecta: → ServicesApi
 * Reglas:     RF-29
 * Estado:     Completo
 */

import { Component, inject, signal } from '@angular/core';
import { ServicesApi } from '../../core/api/services.api';
import { ServiceListItem } from '../../core/models/services.models';
import { OpenTabsService } from '../../core/state/open-tabs.service';
import { StatusChipComponent } from '../../shared/components/status-chip.component';
import { ColonesPipe } from '../../shared/pipes/colones.pipe';
import { CrDatePipe } from '../../shared/pipes/cr-date.pipe';

@Component({
  selector: 'app-ongoing-services-page',
  standalone: true,
  imports: [StatusChipComponent, ColonesPipe, CrDatePipe],
  template: `<table class="dense"><thead><tr><th>Código</th><th>Fallecido</th><th>Sede</th><th>Fecha</th><th>Estado</th><th class="num">Total</th></tr></thead>
    <tbody>@for (s of items(); track s.id) { <tr class="clickable" (click)="tabs.open(s.id, s.code)"><td><strong>{{ s.code }}</strong></td><td>{{ s.deceasedName }}</td><td>{{ s.branchName }}</td><td>{{ s.scheduledAt | crDate }}</td><td><app-status-chip [status]="s.status" /></td><td class="num">{{ s.totalCrc | colones }}</td></tr> }</tbody></table>`,
})
export class OngoingServicesPage {
  readonly tabs = inject(OpenTabsService);
  readonly items = signal<ServiceListItem[]>([]);
  constructor() { inject(ServicesApi).list({ status: 'InProgress', pageSize: 50 }).subscribe(r => this.items.set(r.items)); }
}
