/**
 * Archivo:    frontend/src/app/features/services/service-detail/service-board.component.ts
 * Qué hace:   Tablero del servicio: estado y responsable de cada componente.
 * Recibe:     @Input serviceId.
 * Procesa:    GET board; se actualiza con OfferAwarded/ServiceUpdated; urgentes (< 2 h) resaltados.
 * Devuelve:   Tarjetas por componente con chip de estado, proveedor, precio y hora.
 * Se conecta: ← detail-summary, home · → ServicesApi, OpsHubService
 * Reglas:     RF-17 · RF-19
 * Estado:     Completo
 */

import { Component, DestroyRef, Input, OnChanges, inject, signal } from '@angular/core';
import { takeUntilDestroyed } from '@angular/core/rxjs-interop';
import { ServicesApi } from '../../../core/api/services.api';
import { OpsHubService } from '../../../core/realtime/ops-hub.service';
import { ServiceLine } from '../../../core/models/services.models';
import { SkeletonComponent } from '../../../shared/components/skeleton.component';
import { StatusChipComponent } from '../../../shared/components/status-chip.component';
import { ColonesPipe } from '../../../shared/pipes/colones.pipe';
import { CrDatePipe } from '../../../shared/pipes/cr-date.pipe';

@Component({
  selector: 'app-service-board',
  standalone: true,
  imports: [StatusChipComponent, ColonesPipe, CrDatePipe, SkeletonComponent],
  template: `
    @if (!lines()) { <app-skeleton variant="card" /> } @else {
      <div class="grid">
        @for (l of lines()!; track l.id) {
          <article class="card" [class.urgent]="l.isUrgent" [class.flash]="flashId() === l.id">
            <header><strong>{{ l.componentName }}</strong><app-status-chip [status]="l.status" kind="line" /></header>
            @if (l.imagePath) { <img [src]="'/' + l.imagePath" alt="" /> }
            <p class="muted">{{ l.providerName ? 'Proveedor: ' + l.providerName : (l.status === 'Offered' ? 'Esperando respuesta de proveedores' : 'Sin proveedor externo') }}</p>
            <p>{{ (l.acceptedPriceCrc ?? l.unitPriceCrc) | colones }}@if (l.dueAt) { <span class="muted"> · requerido {{ l.dueAt | crDate:'datetime' }}</span> }</p>
            @if (l.isUrgent) { <p class="warn">Urgente: faltan menos de 2 h y aún no hay proveedor.</p> }
          </article>
        }
      </div>
    }`,
  styles: [`.grid { display: grid; grid-template-columns: repeat(auto-fill, minmax(230px, 1fr)); gap: 12px; } header { display: flex; justify-content: space-between; gap: 8px; } p { margin: 6px 0 0; }
    img { width: 100%; height: 70px; object-fit: cover; border-radius: var(--radius); margin-top: 8px; } .urgent { border-color: var(--warning); } .warn { color: var(--warning); font-size: 12px; }
    .flash { animation: fl 1.6s ease-out; } @keyframes fl { from { background: var(--accent-soft); } to { background: var(--surface); } }`],
})
export class ServiceBoardComponent implements OnChanges {
  private api = inject(ServicesApi);
  private hub = inject(OpsHubService);
  @Input({ required: true }) serviceId!: string;
  readonly lines = signal<ServiceLine[] | null>(null);
  readonly flashId = signal<string | null>(null);

  constructor() {
    const d = inject(DestroyRef);
    this.hub.serviceUpdated$.pipe(takeUntilDestroyed(d)).subscribe(e => e.serviceId === this.serviceId && this.load());
    this.hub.offerAwarded$.pipe(takeUntilDestroyed(d)).subscribe(e => { this.flashId.set(e.lineId); this.load(); setTimeout(() => this.flashId.set(null), 1800); });
  }
  ngOnChanges(): void { this.load(); }
  private load(): void { this.api.board(this.serviceId).subscribe(b => this.lines.set(b.lines)); }
}
