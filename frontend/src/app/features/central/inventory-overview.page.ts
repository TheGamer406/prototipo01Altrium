/**
 * Archivo:    frontend/src/app/features/central/inventory-overview.page.ts
 * Qué hace:   Inventario de cofres de todas las sedes.
 * Recibe:     —
 * Procesa:    StockQuery sin sede.
 * Devuelve:   Matriz sede × modelo.
 * Se conecta: → InventoryApi
 * Reglas:     RF-29
 * Estado:     Completo
 */

import { Component, inject, signal } from '@angular/core';
import { InventoryApi } from '../../core/api/inventory.api';
import { StockRow } from '../../core/models/inventory.models';

@Component({
  selector: 'app-inventory-overview-page',
  standalone: true,
  template: `<p class="muted">Modelos de cofre en el mínimo o agotados, en todas las sedes.</p>
    <table class="dense"><thead><tr><th>Sede</th><th>Modelo</th><th class="num">Existencias</th><th class="num">Mínimo</th><th>Nivel</th></tr></thead>
    <tbody>@for (r of items(); track $index) { <tr><td>{{ r.branchName }}</td><td>{{ r.modelName }} ({{ r.sku }})</td><td class="num">{{ r.qty }}</td><td class="num">{{ r.minQty }}</td><td>{{ r.level === 'out' ? '● Agotado' : '● En el mínimo' }}</td></tr> }</tbody></table>`,
})
export class InventoryOverviewPage {
  readonly items = signal<StockRow[]>([]);
  constructor() { inject(InventoryApi).stock(null).subscribe(r => this.items.set(r.filter(x => x.level !== 'ok'))); }
}
