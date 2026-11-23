/**
 * Archivo:    frontend/src/app/features/driver-app/driver-app.page.ts
 * Qué hace:   App del chofer en marco de teléfono (390 px).
 * Recibe:     Traslado asignado.
 * Procesa:    Botones por hito con lat/lng simulados; interruptor 'Sin señal' encola en vez de enviar.
 * Devuelve:   Hitos enviados o encolados.
 * Se conecta: → offline-queue.service, LogisticsApi
 * Reglas:     RF-22 · RNF-05
 * Estado:     Completo
 */

import { Component, computed, effect, inject, signal } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { LogisticsApi } from '../../core/api/logistics.api';
import { PageHeaderComponent } from '../../core/layout/page-header.component';
import { Transfer } from '../../core/models/logistics.models';
import { BranchContextService } from '../../core/state/branch-context.service';
import { CrDatePipe } from '../../shared/pipes/cr-date.pipe';
import { OfflineQueueService } from './offline-queue.service';

const TYPES = [['Departure', 'Salida de la sede', 'departure_board'], ['Pickup', 'Recogí al fallecido', 'person_pin_circle'], ['ArrivalWake', 'Llegué a la velación', 'church'], ['DepartureWake', 'Salí de la velación', 'directions_car'], ['ArrivalDestination', 'Llegué al destino', 'flag']] as const;

@Component({
  selector: 'app-driver-app-page',
  standalone: true,
  imports: [FormsModule, PageHeaderComponent, CrDatePipe],
  template: `
    <app-page-header title="App del chofer" description="Vista del teléfono del chofer. Con «Sin señal» los hitos se guardan y se envían al volver la conexión." [rf]="['RF-22','RNF-05']" />
    <div class="frame" role="group" aria-label="Teléfono del chofer">
      <div class="status"><span>{{ queue.offline() ? 'Sin señal' : 'Conectado' }}</span><span class="dot" [class.off]="queue.offline()"></span></div>
      <div class="screen">
        <select [ngModel]="transferId()" (ngModelChange)="transferId.set($event)" aria-label="Traslado"><option [ngValue]="null">Elija su traslado…</option>
          @for (t of transfers(); track t.id) { <option [ngValue]="t.id">{{ t.serviceCode }} · {{ t.plate }}</option> }</select>
        <label class="sw"><input type="checkbox" [checked]="queue.offline()" (change)="toggle($any($event.target).checked)" /> Sin señal (simulado)</label>
        @if (queue.pending().length) { <p class="pend">{{ queue.pending().length }} hito(s) en cola, sin enviar</p> }
        @if (current(); as t) {
          <div class="btns">@for (m of types; track m[0]) {
            <button type="button" class="big" [disabled]="done(m[0])" (click)="mark(m[0])"><span class="ms">{{ m[2] }}</span>{{ m[1] }}@if (done(m[0])) { <span class="ok ms">check_circle</span> }</button> }</div>
          <h2>Enviados</h2>
          <ul>@for (h of t.milestones; track h.clientUuid) { <li><strong>{{ label(h.type) }}</strong><span class="muted">{{ h.recordedAt | crDate:'time' }} · {{ h.source === 'Offline' ? 'sin señal' : 'en línea' }}</span></li> }</ul>
        } @else { <p class="muted">Seleccione el traslado asignado para registrar los hitos.</p> }
      </div>
    </div>`,
  styles: [`.frame { width: 390px; max-width: 100%; margin: 0 auto; border: 10px solid #0d1215; border-radius: 36px; background: var(--bg); overflow: hidden; outline: 1px solid var(--border); }
    .status { display: flex; justify-content: space-between; align-items: center; padding: 8px 18px; font-size: 12px; background: #0d1215; color: var(--text-muted); } .dot { width: 8px; height: 8px; border-radius: 50%; background: var(--success); } .dot.off { background: var(--danger); }
    .screen { padding: 14px; display: flex; flex-direction: column; gap: 10px; min-height: 560px; } .sw { display: flex; gap: 8px; align-items: center; font-size: 13px; } .sw input { height: auto; } .pend { color: var(--warning); margin: 0; font-size: 13px; }
    .btns { display: flex; flex-direction: column; gap: 8px; } .big { display: flex; align-items: center; gap: 12px; height: 52px; padding: 0 14px; background: var(--surface-2); border: 1px solid var(--border); border-radius: var(--radius); color: var(--text); font: inherit; font-size: 15px; cursor: pointer; text-align: left; }
    .big:disabled { opacity: .45; cursor: default; } .big .ok { margin-left: auto; color: var(--success); } h2 { margin: 8px 0 0; } ul { list-style: none; padding: 0; margin: 0; } li { display: flex; justify-content: space-between; padding: 6px 0; border-bottom: 1px solid var(--border); font-size: 13px; }`],
})
export class DriverAppPage {
  private api = inject(LogisticsApi);
  private branch = inject(BranchContextService);
  readonly queue = inject(OfflineQueueService);
  readonly types = TYPES;
  readonly transfers = signal<Transfer[]>([]);
  readonly transferId = signal<number | null>(null);
  readonly current = computed(() => this.transfers().find(t => t.id === this.transferId()) ?? null);
  private pos = { lat: 9.9981, lng: -84.1198 };

  constructor() { effect(() => { this.branch.branchId(); this.load(); }); }

  private load(): void { this.api.transfers(this.branch.branchId(), false).subscribe(t => { this.transfers.set(t); if (!this.transferId() && t.length) this.transferId.set(t[0].id); }); }
  label(t: string): string { return TYPES.find(x => x[0] === t)?.[1] ?? t; }
  done(type: string): boolean { return !!this.current()?.milestones.some(m => m.type === type) || this.queue.pending().some(q => q.transferId === this.transferId() && q.item.type === type); }
  toggle(offline: boolean): void { this.queue.setOffline(offline, () => this.load()); }
  mark(type: string): void {
    const c = this.current(); if (!c) return;
    const base = c.lastLat !== null && c.lastLng !== null ? { lat: c.lastLat!, lng: c.lastLng! } : this.pos;
    this.pos = { lat: base.lat + (Math.random() - 0.4) * 0.01, lng: base.lng + (Math.random() - 0.5) * 0.01 };
    this.queue.record(c.id, type, this.pos.lat, this.pos.lng, () => this.load());
  }
}
