/**
 * Archivo:    frontend/src/app/shared/components/skeleton.component.ts
 * Qué hace:   Placeholder de carga.
 * Recibe:     @Input rows, variant ('table'|'card'|'chart').
 * Procesa:    —
 * Devuelve:   Bloques animados suaves (150 ms), nunca pantalla en blanco.
 * Se conecta: ← todas las páginas mientras cargan
 * Estado:     Completo
 */

import { Component, Input } from '@angular/core';

@Component({
  selector: 'app-skeleton',
  standalone: true,
  template: `
    @if (variant === 'card') { <div class="block card-b"></div> }
    @else if (variant === 'chart') { <div class="block chart-b"></div> }
    @else { @for (r of rowsArray; track $index) { <div class="block row-b"></div> } }`,
  styles: [`
    .block { background: linear-gradient(90deg, var(--surface) 0%, var(--surface-2) 50%, var(--surface) 100%); background-size: 200% 100%;
      animation: sk 1.4s ease-in-out infinite; border-radius: var(--radius); }
    .row-b { height: var(--row-h); margin-bottom: 4px; } .card-b { height: 88px; } .chart-b { height: 260px; }
    @keyframes sk { from { background-position: 200% 0; } to { background-position: -200% 0; } }
    @media (prefers-reduced-motion: reduce) { .block { animation: none; } }`],
})
export class SkeletonComponent {
  @Input() rows = 6;
  @Input() variant: 'table' | 'card' | 'chart' = 'table';
  get rowsArray(): number[] { return Array.from({ length: this.rows }); }
}
