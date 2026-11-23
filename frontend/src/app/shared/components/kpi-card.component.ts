/**
 * Archivo:    frontend/src/app/shared/components/kpi-card.component.ts
 * Qué hace:   Ficha de indicador.
 * Recibe:     @Input label, value, hint?, trend?
 * Procesa:    —
 * Devuelve:   Tarjeta con número tabular.
 * Se conecta: ← home, central-view, analytics
 * Estado:     Completo
 */

import { Component, Input } from '@angular/core';

@Component({
  selector: 'app-kpi-card',
  standalone: true,
  template: `
    <div class="kpi">
      <span class="label">{{ label }}</span>
      <span class="value">{{ value }}</span>
      @if (hint) { <span class="hint">{{ hint }}</span> }
    </div>`,
  styles: [`.kpi { background: var(--surface); border: 1px solid var(--border); border-radius: var(--radius); padding: 14px 16px; display: flex; flex-direction: column; gap: 4px; min-width: 150px; }
    .label { color: var(--text-muted); font-size: 12px; } .value { font-size: 24px; font-weight: 600; } .hint { color: var(--text-muted); font-size: 12px; }`],
})
export class KpiCardComponent {
  @Input() label = '';
  @Input() value: string | number = '';
  @Input() hint?: string;
  @Input() trend?: 'up' | 'down' | 'flat';
}
