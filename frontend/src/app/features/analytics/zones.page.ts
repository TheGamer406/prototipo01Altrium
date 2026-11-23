/**
 * Archivo:    frontend/src/app/features/analytics/zones.page.ts
 * Qué hace:   Zonas y temporada.
 * Recibe:     Año.
 * Procesa:    AnalyticsApi.zones.
 * Devuelve:   Mapa de calor zona × mes (pico dic-ene).
 * Se conecta: → AnalyticsApi
 * Reglas:     RF-34
 * Estado:     Completo
 */

import { Component, inject, signal } from '@angular/core';
import { NgxEchartsDirective } from 'ngx-echarts';
import { AnalyticsApi } from '../../core/api/analytics.api';
import { SkeletonComponent } from '../../shared/components/skeleton.component';

@Component({
  selector: 'app-zones-page',
  standalone: true,
  imports: [NgxEchartsDirective, SkeletonComponent],
  template: `@if (!ready()) { <app-skeleton variant="chart" /> } @else { <section class="card"><h2>Servicios por zona y mes</h2><p class="muted">Se nota el pico de diciembre y enero.</p><div echarts [options]="chart()" theme="acompana" class="chart"></div></section> }`,
  styles: [`.chart { height: 320px; }`],
})
export class ZonesPage {
  readonly ready = signal(false);
  readonly chart = signal<any>({});
  constructor() {
    inject(AnalyticsApi).zones().subscribe(r => {
      const months = [...new Set(r.cells.map(c => c.month))].sort();
      const zones = ['GAM', 'Fuera'];
      const names: Record<string, string> = { GAM: 'GAM', Fuera: 'Fuera del GAM' };
      this.chart.set({ grid: { left: 48, right: 16, top: 40, bottom: 28 }, tooltip: { trigger: 'axis' }, legend: { top: 0 }, xAxis: { type: 'category', data: months },
        yAxis: { type: 'value' }, series: zones.map(z => ({ name: names[z], type: 'bar', stack: 'z', data: months.map(m => r.cells.find(c => c.zone === z && c.month === m)?.count ?? 0) })) });
      this.ready.set(true);
    });
  }
}
