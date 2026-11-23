/**
 * Archivo:    frontend/src/app/features/services/new-service/total-panel.component.ts
 * Qué hace:   Panel lateral fijo con el total en vivo.
 * Recibe:     Líneas del formulario.
 * Procesa:    Mismo cálculo que InvoiceCalculator (IVA 13 % menos exoneración por línea).
 * Devuelve:   Subtotal, exento, IVA y total en colones.
 * Se conecta: ← new-service-wizard
 * Reglas:     RF-05 · RF-10
 * Estado:     Completo
 */

import { Component, Input, computed, signal } from '@angular/core';
import { ColonesPipe } from '../../../shared/pipes/colones.pipe';
import { MathLine, calcTotals } from './invoice-math';

export interface PanelLine extends MathLine { name: string; }

@Component({
  selector: 'app-total-panel',
  standalone: true,
  imports: [ColonesPipe],
  template: `
    <aside class="card panel" aria-label="Total del servicio">
      <h2>Total</h2>
      <ul>@for (l of lines(); track $index) { <li><span>{{ l.qty }} × {{ l.name }}</span><span>{{ l.qty * l.unitPriceCrc | colones }}</span></li> }</ul>
      <dl>
        <dt>Subtotal</dt><dd>{{ totals().subtotal | colones }}</dd>
        <dt>Exonerado</dt><dd>{{ totals().exempt | colones }}</dd>
        <dt>IVA 13 %</dt><dd>{{ totals().tax | colones }}</dd>
        <dt class="big">Total</dt><dd class="big">{{ totals().total | colones }}</dd>
      </dl>
    </aside>`,
  styles: [`.panel { position: sticky; top: 0; } ul { list-style: none; margin: 8px 0; padding: 0; max-height: 280px; overflow: auto; font-size: 12px; }
    li { display: flex; justify-content: space-between; gap: 8px; padding: 3px 0; color: var(--text-muted); }
    dl { display: grid; grid-template-columns: 1fr auto; gap: 4px 12px; margin: 8px 0 0; border-top: 1px solid var(--border); padding-top: 8px; } dd { margin: 0; text-align: right; }
    .big { font-size: 16px; font-weight: 600; }`],
})
export class TotalPanelComponent {
  private _lines = signal<PanelLine[]>([]);
  @Input() set value(v: PanelLine[]) { this._lines.set(v ?? []); }
  readonly lines = this._lines.asReadonly();
  readonly totals = computed(() => calcTotals(this._lines()));
}
