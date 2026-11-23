/**
 * Archivo:    frontend/src/app/features/transfers/transfers-map.page.ts
 * Qué hace:   Mapa de carrozas (Leaflet + OSM).
 * Recibe:     Sede.
 * Procesa:    Marcadores con última posición; demorados en --warning.
 * Devuelve:   Mapa + lista lateral.
 * Se conecta: → LogisticsApi
 * Reglas:     RF-23 · RF-24
 * Estado:     Completo
 */

import { AfterViewInit, Component, ElementRef, OnDestroy, ViewChild, effect, inject, signal } from '@angular/core';
import { RouterLink } from '@angular/router';
import * as L from 'leaflet';
import { LogisticsApi } from '../../core/api/logistics.api';
import { PageHeaderComponent } from '../../core/layout/page-header.component';
import { Transfer } from '../../core/models/logistics.models';
import { OpsHubService } from '../../core/realtime/ops-hub.service';
import { BranchContextService } from '../../core/state/branch-context.service';
import { EmptyStateComponent } from '../../shared/components/empty-state.component';

const ST: Record<string, string> = { Planned: 'Planificado', OnRoute: 'En ruta', Done: 'Finalizado' };

@Component({
  selector: 'app-transfers-map-page',
  standalone: true,
  imports: [RouterLink, PageHeaderComponent, EmptyStateComponent],
  template: `
    <app-page-header title="Traslados" description="Carrozas en ruta con su última posición. Los traslados demorados se resaltan." [rf]="['RF-23','RF-24']" />
    <div class="layout">
      <div #map class="map card" role="img" aria-label="Mapa de traslados"></div>
      <div class="card list">
        @if (!items().length) { <app-empty-state icon="local_shipping" text="No hay traslados activos." /> }
        <table class="dense"><tbody>@for (t of items(); track t.id) { <tr [class.late]="t.delayed"><td><a [routerLink]="['/traslados', t.id]"><strong>{{ t.serviceCode }}</strong></a><br><span class="muted">{{ t.plate }} · {{ t.driverName }}</span></td>
          <td>{{ st[t.status] }}@if (t.delayed) { <br><span class="warn">● Demorado</span> }</td></tr> }</tbody></table>
      </div>
    </div>`,
  styles: [`.layout { display: grid; grid-template-columns: 1.4fr 1fr; gap: 12px; } .map { height: 520px; padding: 0; } .list { max-height: 520px; overflow: auto; padding: 0 8px; } .warn { color: var(--warning); font-size: 12px; } .late td:last-child { color: var(--warning); }
    @media (max-width: 1024px) { .layout { grid-template-columns: 1fr; } .map { height: 360px; } }`],
})
export class TransfersMapPage implements AfterViewInit, OnDestroy {
  private api = inject(LogisticsApi);
  private branch = inject(BranchContextService);
  @ViewChild('map') mapEl!: ElementRef<HTMLDivElement>;
  readonly st = ST;
  readonly items = signal<Transfer[]>([]);
  private map?: L.Map;
  private layer = L.layerGroup();
  private ready = signal(false);
  private sub = inject(OpsHubService).serviceUpdated$.subscribe(() => this.load());
  private timer = setInterval(() => this.load(), 15000);

  constructor() { effect(() => { this.branch.branchId(); this.ready(); this.load(); }); }

  ngAfterViewInit(): void {
    this.map = L.map(this.mapEl.nativeElement, { attributionControl: false }).setView([9.9, -84.1], 8);
    this.layer.addTo(this.map); this.ready.set(true);
  }
  ngOnDestroy(): void { this.sub.unsubscribe(); clearInterval(this.timer); this.map?.remove(); }

  private load(): void {
    this.api.transfers(this.branch.branchId(), false).subscribe(list => {
      this.items.set(list); this.layer.clearLayers();
      const pts: L.LatLngExpression[] = [];
      for (const t of list.filter(x => x.lastLat !== null)) {
        const color = t.delayed ? '#be9f5e' : '#5fafc8';
        const trail = t.milestones.filter(m => m.lat != null).map(m => [m.lat!, m.lng!] as L.LatLngExpression);
        if (trail.length > 1) L.polyline(trail, { color, weight: 2, opacity: 0.5, dashArray: '4 4' }).addTo(this.layer);
        L.circleMarker([t.lastLat!, t.lastLng!], { radius: 9, color, fillColor: color, fillOpacity: 0.6, weight: 2 })
          .bindTooltip(`<strong>${t.serviceCode}</strong><br>${t.plate} · ${t.driverName}${t.delayed ? '<br>Demorado' : ''}`).addTo(this.layer);
        pts.push([t.lastLat!, t.lastLng!]);
      }
      if (pts.length) this.map?.fitBounds(L.latLngBounds(pts), { padding: [40, 40], maxZoom: 12 });
    });
  }
}
