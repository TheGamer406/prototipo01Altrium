/**
 * Archivo:    frontend/src/app/features/services/service-detail/service-detail.page.ts
 * Qué hace:   Detalle del servicio con pestañas internas.
 * Recibe:     Ruta :id.
 * Procesa:    Carga ServiceDetailDto; registra la pestaña ERP; escucha ServiceUpdated del hub para
 *               recargar.
 * Devuelve:   Encabezado (código, estado, sede) + pestañas Resumen · Paquete · Documentos · Factura ·
 *               Proveedores · Logística · Mensajes · Bitácora.
 * Se conecta: → ServicesApi, OpenTabsService, OpsHubService
 * Reglas:     RF-01 a RF-13 · RF-19
 * Estado:     Completo
 */

import { Component, DestroyRef, inject, signal } from '@angular/core';
import { takeUntilDestroyed } from '@angular/core/rxjs-interop';
import { ActivatedRoute } from '@angular/router';
import { ServicesApi } from '../../../core/api/services.api';
import { OpsHubService } from '../../../core/realtime/ops-hub.service';
import { ServiceDetail } from '../../../core/models/services.models';
import { OpenTabsService } from '../../../core/state/open-tabs.service';
import { SkeletonComponent } from '../../../shared/components/skeleton.component';
import { StatusChipComponent } from '../../../shared/components/status-chip.component';
import { RfTagComponent } from '../../../shared/components/rf-tag.component';
import { DetailAuditTab } from './detail-audit.tab';
import { DetailDocumentsTab } from './detail-documents.tab';
import { DetailInvoiceTab } from './detail-invoice.tab';
import { DetailLogisticsTab } from './detail-logistics.tab';
import { DetailMessagesTab } from './detail-messages.tab';
import { DetailPackageTab } from './detail-package.tab';
import { DetailProvidersTab } from './detail-providers.tab';
import { DetailSummaryTab } from './detail-summary.tab';

const TABS = [['summary', 'Resumen'], ['package', 'Paquete'], ['documents', 'Documentos'], ['invoice', 'Factura'], ['providers', 'Proveedores'], ['logistics', 'Logística'], ['messages', 'Mensajes'], ['audit', 'Bitácora']] as const;

@Component({
  selector: 'app-service-detail-page',
  standalone: true,
  imports: [SkeletonComponent, StatusChipComponent, RfTagComponent, DetailSummaryTab, DetailPackageTab, DetailDocumentsTab, DetailInvoiceTab, DetailProvidersTab, DetailLogisticsTab, DetailMessagesTab, DetailAuditTab],
  template: `
    @if (!s()) { <app-skeleton variant="card" /> } @else {
      <header class="head"><h1>{{ s()!.code }}</h1><app-status-chip [status]="s()!.status" />
        <span class="muted">{{ s()!.deceased.fullName }} · {{ s()!.branchName }}</span><app-rf-tag [rf]="['RF-01','RF-13','RF-19']" /></header>
      <div class="tabs" role="tablist">@for (t of tabs; track t[0]) { <button type="button" role="tab" [class.on]="tab() === t[0]" [attr.aria-selected]="tab() === t[0]" (click)="tab.set(t[0])">{{ t[1] }}</button> }</div>
      @switch (tab()) {
        @case ('summary') { <app-detail-summary-tab [s]="s()!" /> }
        @case ('package') { <app-detail-package-tab [s]="s()!" (changed)="load()" /> }
        @case ('documents') { <app-detail-documents-tab [s]="s()!" (changed)="load()" /> }
        @case ('invoice') { <app-detail-invoice-tab [s]="s()!" (changed)="load()" /> }
        @case ('providers') { <app-detail-providers-tab [serviceId]="s()!.id" /> }
        @case ('logistics') { <app-detail-logistics-tab [s]="s()!" /> }
        @case ('messages') { <app-detail-messages-tab [serviceId]="s()!.id" /> }
        @case ('audit') { <app-detail-audit-tab [serviceId]="s()!.id" /> }
      }
    }`,
  styles: [`.head { display: flex; align-items: center; gap: 12px; margin-bottom: 12px; } .tabs { display: flex; gap: 2px; border-bottom: 1px solid var(--border); margin-bottom: 16px; }
    .tabs button { background: none; border: none; border-bottom: 2px solid transparent; color: var(--text-muted); font: inherit; height: 36px; padding: 0 14px; cursor: pointer; } .tabs .on { color: var(--text); border-color: var(--accent); }`],
})
export class ServiceDetailPage {
  private api = inject(ServicesApi);
  private tabsSvc = inject(OpenTabsService);
  private id = inject(ActivatedRoute).snapshot.paramMap.get('id')!;
  readonly tabs = TABS;
  readonly tab = signal<(typeof TABS)[number][0]>('summary');
  readonly s = signal<ServiceDetail | null>(null);

  constructor() {
    this.load();
    inject(OpsHubService).serviceUpdated$.pipe(takeUntilDestroyed(inject(DestroyRef))).subscribe(e => e.serviceId === this.id && this.load());
  }

  load(): void { this.api.get(this.id).subscribe(d => { this.s.set(d); this.tabsSvc.register(d.id, d.code); }); }
}
