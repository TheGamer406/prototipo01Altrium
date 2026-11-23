/**
 * Archivo:    frontend/src/app/features/analytics/efficiency.page.ts
 * Qué hace:   Eficiencia.
 * Recibe:     Rango.
 * Procesa:    AnalyticsApi.stageTimes.
 * Devuelve:   Barras horizontales por etapa + etapa más lenta por sede.
 * Se conecta: → AnalyticsApi
 * Reglas:     RF-33
 * Estado:     Completo
 */

import { Component, inject, signal } from '@angular/core';
import { NgxEchartsDirective } from 'ngx-echarts';
import { AnalyticsApi } from '../../core/api/analytics.api';
import { StageTimes } from '../../core/models/analytics.models';
import { SkeletonComponent } from '../../shared/components/skeleton.component';

const LABEL: Record<string, string> = { Registered: 'Registro', Invoicing: 'Facturación', Paid: 'Pago', InProgress: 'En curso', Completed: 'Fin' };

@Component({
  selector: 'app-efficiency-page',
  standalone: true,
  imports: [NgxEchartsDirective, SkeletonComponent],
  template: `@if (!d()) { <app-skeleton variant="chart" /> } @else {
    <div class="grid"><section class="card"><h2>Horas promedio entre estados</h2><div echarts [options]="chart()" theme="acompana" class="chart"></div></section>
      <section class="card"><h2>Etapa más lenta por sede</h2><table class="dense"><thead><tr><th>Sede</th><th>Etapa</th><th class="num">Horas</th></tr></thead>
        <tbody>@for (s of d()!.slowestByBranch; track s.branch) { <tr><td>{{ s.branch }}</td><td>{{ label(s.stage) }}</td><td class="num">{{ s.avgHours }}</td></tr> }</tbody></table></section></div> }`,
  styles: [`.grid { display: grid; grid-template-columns: 1fr 1fr; gap: 12px; } .chart { height: 320px; }`],
})
export class EfficiencyPage {
  readonly d = signal<StageTimes | null>(null);
  readonly chart = signal<any>({});
  constructor() {
    inject(AnalyticsApi).stageTimes().subscribe(r => {
      this.d.set(r);
      this.chart.set({ grid: { left: 150, right: 24, top: 8, bottom: 24 }, tooltip: { trigger: 'axis' }, xAxis: { type: 'value', name: 'horas' },
        yAxis: { type: 'category', inverse: true, data: r.stages.map(s => `${LABEL[s.from]} → ${LABEL[s.to]}`) }, series: [{ type: 'bar', data: r.stages.map(s => s.avgHours), barMaxWidth: 22 }] });
    });
  }
  label(stage: string): string { return stage.split(' → ').map(x => LABEL[x] ?? x).join(' → '); }
}
