/**
 * Archivo:    frontend/src/app/features/inventory/stock.page.ts
 * Qué hace:   Stock por sede y modelo.
 * Recibe:     Sede.
 * Procesa:    Niveles ok/mín/agotado con chip.
 * Devuelve:   Tabla con imagen del cofre.
 * Se conecta: → InventoryApi, OpsHubService (StockChanged)
 * Reglas:     RF-26 · RF-27
 * Estado:     Completo
 */

import { Component, effect, inject, signal } from '@angular/core';
import { InventoryApi } from '../../core/api/inventory.api';
import { StockRow } from '../../core/models/inventory.models';
import { BranchContextService } from '../../core/state/branch-context.service';
import { OpsHubService } from '../../core/realtime/ops-hub.service';

@Component({
  selector: 'app-stock-page',
  standalone: true,
  template: `<div class="grid">@for (r of rows(); track $index) {
    <article class="card" [class.min]="r.level === 'min'" [class.out]="r.level === 'out'"><img [src]="'/' + r.imagePath" alt="" /><strong>{{ r.modelName }}</strong><span class="muted">{{ r.sku }}@if (!branch.branchId()) { · {{ r.branchName }} }</span>
      <span class="q">{{ r.qty }} <small class="muted">/ mínimo {{ r.minQty }}</small></span><span class="lvl">{{ r.level === 'ok' ? '● Normal' : r.level === 'min' ? '● En el mínimo' : '● Agotado' }}</span></article> }</div>`,
  styles: [`.grid { display: grid; grid-template-columns: repeat(auto-fill, minmax(190px, 1fr)); gap: 12px; } article { display: flex; flex-direction: column; gap: 4px; padding: 10px; } img { height: 90px; object-fit: contain; background: var(--bg); border-radius: var(--radius); }
    .q { font-size: 22px; font-weight: 600; } .lvl { font-size: 12px; color: var(--success); } .min .lvl { color: var(--warning); } .out .lvl { color: var(--danger); } .out { border-color: var(--danger); } .min { border-color: var(--warning); }`],
})
export class StockPage {
  private api = inject(InventoryApi);
  readonly branch = inject(BranchContextService);
  readonly rows = signal<StockRow[]>([]);
  constructor() {
    effect(() => { const b = this.branch.branchId(); this.api.stock(b).subscribe(r => this.rows.set(r.slice(0, 120))); });
    inject(OpsHubService).stockChanged$.subscribe(() => this.api.stock(this.branch.branchId()).subscribe(r => this.rows.set(r.slice(0, 120))));
  }
}
