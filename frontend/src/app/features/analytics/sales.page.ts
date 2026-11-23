/**
 * Archivo:    frontend/src/app/features/analytics/sales.page.ts
 * Qué hace:   Ventas.
 * Recibe:     Rango y sede (en la URL).
 * Procesa:    AnalyticsApi.sales.
 * Devuelve:   Barras por paquete, línea por mes, tabla por sede.
 * Se conecta: → AnalyticsApi, chart-theme
 * Reglas:     RF-32
 * Estado:     Completo
 */

import { Component, inject, signal } from '@angular/core';
import { NgxEchartsDirective } from 'ngx-echarts';
import { AnalyticsApi } from '../../core/api/analytics.api';
import { SalesResponse } from '../../core/models/analytics.models';
import { SkeletonComponent } from '../../shared/components/skeleton.component';
import { KpiStripComponent } from './kpi-strip.component';

const M = (n: number) => `₡${Math.round(n / 1e6)} M`;

@Component({
  selector: 'app-sales-page',
  standalone: true,
  imports: [NgxEchartsDirective, KpiStripComponent, SkeletonComponent],
  template: `@if (!d()) { <app-skeleton variant="chart" /> } @else {
    <app-kpi-strip [kpi]="d()!.kpis" />
    <div class="grid">
      <section class="card"><h2>Ventas por mes</h2><div echarts [options]="month()" theme="acompana" class="chart"></div></section>
      <section class="card"><h2>Ventas por paquete</h2><div echarts [options]="pack()" theme="acompana" class="chart"></div></section>
      <section class="card wide"><h2>Ventas por sede (10 principales)</h2><div echarts [options]="branch()" theme="acompana" class="chart"></div></section>
    </div> }`,
  styles: [`.grid { display: grid; grid-template-columns: 1fr 1fr; gap: 12px; } .wide { grid-column: 1 / -1; } .chart { height: 280px; }
    @media (max-width: 1024px) { .grid { grid-template-columns: 1fr; } }`],
})
export class SalesPage {
  readonly d = signal<SalesResponse | null>(null);
  readonly month = signal<any>({}); readonly pack = signal<any>({}); readonly branch = signal<any>({});
  constructor() {
    inject(AnalyticsApi).sales().subscribe(r => {
      this.d.set(r);
      this.month.set({ grid: { left: 56, right: 16, top: 16, bottom: 28 }, tooltip: { trigger: 'axis', valueFormatter: (v: number) => M(v) }, xAxis: { type: 'category', data: r.sales.byMonth.map(m => m.month) },
        yAxis: { type: 'value', axisLabel: { formatter: (v: number) => M(v) } }, series: [{ type: 'bar', data: r.sales.byMonth.map(m => Math.round(m.totalCrc)), barMaxWidth: 28 }] });
      this.pack.set({ tooltip: { trigger: 'item', valueFormatter: (v: number) => M(v) }, legend: { bottom: 0 }, series: [{ type: 'pie', radius: ['45%', '70%'], label: { show: false }, data: r.sales.byPackage.map(p => ({ name: p.package, value: Math.round(p.totalCrc) })) }] });
      const top = r.sales.byBranch.slice(0, 10);
      this.branch.set({ grid: { left: 130, right: 24, top: 8, bottom: 24 }, tooltip: { trigger: 'axis', valueFormatter: (v: number) => M(v) }, xAxis: { type: 'value', axisLabel: { formatter: (v: number) => M(v) } },
        yAxis: { type: 'category', inverse: true, data: top.map(b => b.branch) }, series: [{ type: 'bar', data: top.map(b => Math.round(b.totalCrc)), barMaxWidth: 16 }] });
    });
  }
}
