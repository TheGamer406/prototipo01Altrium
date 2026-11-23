/**
 * Archivo:    frontend/src/app/features/central/branches-map.page.ts
 * Qué hace:   Sedes en mapa + tabla.
 * Recibe:     OverviewDto.
 * Procesa:    Leaflet con 28 marcadores (tamaño por servicios en curso); tabla ordenable; refresco por
 *               hub.
 * Devuelve:   Mapa y tabla.
 * Se conecta: → OversightApi, OpsHubService
 * Reglas:     RF-29 · RNF-06
 * Estado:     Completo
 */

import { AfterViewInit, Component, ElementRef, OnDestroy, ViewChild, inject, signal } from '@angular/core';
import * as L from 'leaflet';
import { OversightApi } from '../../core/api/oversight.api';
import { Overview } from '../../core/models/oversight.models';
import { OpsHubService } from '../../core/realtime/ops-hub.service';
import { KpiCardComponent } from '../../shared/components/kpi-card.component';
import { SkeletonComponent } from '../../shared/components/skeleton.component';

@Component({
  selector: 'app-branches-map-page',
  standalone: true,
  imports: [KpiCardComponent, SkeletonComponent],
  template: `
    @if (data(); as d) {
      <div class="kpis"><app-kpi-card label="Servicios en curso" [value]="d.kpis.ongoing" /><app-kpi-card label="Capillas ocupadas" [value]="d.kpis.chapelsBusy" />
        <app-kpi-card label="Cofres bajo mínimo" [value]="d.kpis.lowStock" /><app-kpi-card label="Alertas abiertas" [value]="d.kpis.openAlerts" /></div>
    } @else { <app-skeleton variant="card" /> }
    <div class="layout">
      <div #map class="map card" role="img" aria-label="Mapa de sedes"></div>
      <div class="card list"><table class="dense"><thead><tr><th>Sede</th><th class="num">En curso</th><th class="num">Capillas</th><th class="num">Bajo mín.</th></tr></thead>
        <tbody>@for (b of data()?.branches ?? []; track b.id) { <tr [class.hot]="b.lowStock > 0"><td>{{ b.name }}</td><td class="num">{{ b.ongoing }}</td><td class="num">{{ b.chapelsBusy }}/{{ b.chapelsTotal }}</td><td class="num">{{ b.lowStock }}</td></tr> }</tbody></table></div>
    </div>`,
  styles: [`.kpis { display: flex; gap: 12px; margin-bottom: 16px; flex-wrap: wrap; } .layout { display: grid; grid-template-columns: 1.3fr 1fr; gap: 12px; } .map { height: 520px; padding: 0; background: var(--surface); }
    .list { max-height: 520px; overflow: auto; padding: 0 8px; } .hot td:last-child { color: var(--warning); }
    @media (max-width: 1024px) { .layout { grid-template-columns: 1fr; } } @media (max-width: 1024px) { .map { height: 360px; } }`],
})
export class BranchesMapPage implements AfterViewInit, OnDestroy {
  private api = inject(OversightApi);
  private hub = inject(OpsHubService);
  @ViewChild('map') mapEl!: ElementRef<HTMLDivElement>;
  readonly data = signal<Overview | null>(null);
  private map?: L.Map;
  private layer = L.layerGroup();
  private subs = [this.hub.serviceUpdated$.subscribe(() => this.load()), this.hub.stockChanged$.subscribe(() => this.load()), this.hub.alertRaised$.subscribe(() => this.load())];

  ngAfterViewInit(): void {
    this.map = L.map(this.mapEl.nativeElement, { zoomControl: true, attributionControl: false }).setView([9.75, -84.1], 7);
    this.layer.addTo(this.map);
    this.load();
  }
  ngOnDestroy(): void { this.subs.forEach(s => s.unsubscribe()); this.map?.remove(); }

  private load(): void {
    this.api.overview().subscribe(o => {
      this.data.set(o); this.layer.clearLayers();
      const pts: L.LatLngExpression[] = [];
      for (const b of o.branches) {
        const color = b.lowStock > 0 ? '#be9f5e' : b.ongoing > 0 ? '#5fafc8' : '#7a8fa6';
        L.circleMarker([b.lat, b.lng], { radius: 6 + Math.min(b.ongoing, 6) * 2, color, fillColor: color, fillOpacity: 0.45, weight: 1.5 })
          .bindTooltip(`<strong>${b.name}</strong><br>${b.ongoing} en curso · capillas ${b.chapelsBusy}/${b.chapelsTotal}<br>${b.lowStock} cofres bajo mínimo`).addTo(this.layer);
        pts.push([b.lat, b.lng]);
      }
      if (pts.length) this.map?.fitBounds(L.latLngBounds(pts), { padding: [24, 24] });
    });
  }
}
