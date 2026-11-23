/**
 * Archivo:    frontend/src/app/features/cremation/cremation-board.page.ts
 * Qué hace:   Kanban del crematorio (4 columnas).
 * Recibe:     —
 * Procesa:    Mover tarjeta al siguiente estado (solo hacia adelante); al entregar pide quién recibe;
 *               hub CremationMoved.
 * Devuelve:   Tablero.
 * Se conecta: → LogisticsApi, OpsHubService
 * Reglas:     RF-21
 * Estado:     Completo
 */

import { Component, inject, signal } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { MatSnackBar } from '@angular/material/snack-bar';
import { LogisticsApi } from '../../core/api/logistics.api';
import { PageHeaderComponent } from '../../core/layout/page-header.component';
import { CremationBoard, CremationCard } from '../../core/models/logistics.models';
import { OpsHubService } from '../../core/realtime/ops-hub.service';
import { CrDatePipe } from '../../shared/pipes/cr-date.pipe';

const COLS = [['waiting', 'En espera', 'Mover a horno'], ['inOven', 'En horno', 'Marcar empacado'], ['packed', 'Empacado', 'Entregar cenizas'], ['delivered', 'Entregado', '']] as const;

@Component({
  selector: 'app-cremation-board-page',
  standalone: true,
  imports: [FormsModule, PageHeaderComponent, CrDatePipe],
  template: `
    <app-page-header title="Cremación" description="Cola del crematorio central (San José). Mueva cada tarjeta al siguiente estado." [rf]="['RF-21']" />
    <div class="kanban">@for (c of cols; track c[0]) {
      <section><h2>{{ c[1] }} <span class="muted">{{ list(c[0]).length }}</span></h2>
        @for (card of list(c[0]); track card.id) {
          <article class="card"><strong>{{ card.serviceCode }}</strong><span>{{ card.deceasedName }}</span><span class="muted">Turno #{{ card.queuePosition }} · {{ card.waitingAt | crDate }}</span>
            @if (card.operator) { <span class="muted">Operador: {{ card.operator }}</span> }
            @if (card.receivedBy) { <span class="muted">Recibió: {{ card.receivedBy }}</span> }
            @if (c[0] === 'packed') { <input [(ngModel)]="receiver[card.id]" placeholder="Quién recibe las cenizas" [attr.aria-label]="'Receptor de ' + card.serviceCode" /> }
            @if (c[2]) { <button class="btn primary" type="button" (click)="advance(card)">{{ c[2] }}</button> }
          </article> }
        @if (!list(c[0]).length) { <p class="muted empty">Sin tarjetas</p> }
      </section> }</div>`,
  styles: [`.kanban { display: grid; grid-template-columns: repeat(4, 1fr); gap: 12px; align-items: start; } section { background: var(--bg); border: 1px solid var(--border); border-radius: var(--radius); padding: 10px; min-height: 240px; }
    article { display: flex; flex-direction: column; gap: 4px; margin-top: 8px; padding: 10px; } h2 { margin: 0; } .empty { text-align: center; margin-top: 24px; }`],
})
export class CremationBoardPage {
  private api = inject(LogisticsApi);
  private snack = inject(MatSnackBar);
  readonly cols = COLS;
  readonly board = signal<CremationBoard | null>(null);
  receiver: Record<number, string> = {};
  private operatorId = 0; // 0 = el servidor asigna al operador del turno
  constructor() {
    this.load();
    inject(OpsHubService).cremationMoved$.subscribe(() => this.load());
  }
  list(key: string): CremationCard[] { return (this.board() as unknown as Record<string, CremationCard[]>)?.[key] ?? []; }
  private load(): void { this.api.cremations().subscribe(b => this.board.set(b)); }
  advance(c: CremationCard): void {
    if (c.status === 'Packed' && !this.receiver[c.id]?.trim()) { this.snack.open('Indique quién recibe las cenizas.', 'Cerrar', { duration: 4000, panelClass: 'error' }); return; }
    this.api.advanceCremation(c.id, { operatorId: this.operatorId, receivedBy: this.receiver[c.id] }).subscribe(() => this.load());
  }
}
