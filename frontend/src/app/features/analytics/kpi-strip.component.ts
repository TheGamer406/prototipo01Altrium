/**
 * Archivo:    frontend/src/app/features/analytics/kpi-strip.component.ts
 * Qué hace:   Fichas de indicadores arriba de la analítica.
 * Recibe:     KpiDto.
 * Procesa:    —
 * Devuelve:   Servicios del mes, ticket promedio, % aseguradora, registro → pago.
 * Se conecta: ← páginas de analítica
 * Reglas:     RF-32
 * Estado:     Completo
 */

import { Component, Input } from '@angular/core';
import { Kpi } from '../../core/models/analytics.models';
import { KpiCardComponent } from '../../shared/components/kpi-card.component';
import { ColonesPipe } from '../../shared/pipes/colones.pipe';

@Component({
  selector: 'app-kpi-strip',
  standalone: true,
  imports: [KpiCardComponent],
  template: `@if (kpi) { <div class="row">
    <app-kpi-card label="Servicios este mes" [value]="kpi.servicesThisMonth" />
    <app-kpi-card label="Ticket promedio" [value]="money(kpi.avgTicketCrc)" />
    <app-kpi-card label="Pagados por aseguradora" [value]="kpi.insurerPaidPct + ' %'" />
    <app-kpi-card label="Registro → pago" [value]="kpi.avgRegisterToPaidHours + ' h'" hint="promedio" /></div> }`,
  styles: [`.row { display: flex; gap: 12px; flex-wrap: wrap; margin-bottom: 16px; }`],
})
export class KpiStripComponent {
  @Input() kpi: Kpi | null = null;
  private fmt = new ColonesPipe();
  money(v: number): string { return this.fmt.transform(v); }
}
