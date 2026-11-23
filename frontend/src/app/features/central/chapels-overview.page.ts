/**
 * Archivo:    frontend/src/app/features/central/chapels-overview.page.ts
 * Qué hace:   Ocupación de capillas por sede.
 * Recibe:     —
 * Procesa:    Overview.
 * Devuelve:   Barras/tabla.
 * Se conecta: → OversightApi
 * Reglas:     RF-29
 * Estado:     Completo
 */

import { Component, inject, signal } from '@angular/core';
import { OversightApi } from '../../core/api/oversight.api';
import { BranchOverview } from '../../core/models/oversight.models';

@Component({
  selector: 'app-chapels-overview-page',
  standalone: true,
  template: `<table class="dense"><thead><tr><th>Sede</th><th>Zona</th><th class="num">Capillas ocupadas</th><th class="num">Capillas totales</th></tr></thead>
    <tbody>@for (b of items(); track b.id) { <tr><td>{{ b.name }}</td><td>{{ b.zone }}</td><td class="num">{{ b.chapelsBusy }}</td><td class="num">{{ b.chapelsTotal }}</td></tr> }</tbody></table>`,
})
export class ChapelsOverviewPage {
  readonly items = signal<BranchOverview[]>([]);
  constructor() { inject(OversightApi).overview().subscribe(o => this.items.set(o.branches.filter(b => b.chapelsTotal > 0))); }
}
