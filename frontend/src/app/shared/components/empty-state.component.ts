/**
 * Archivo:    frontend/src/app/shared/components/empty-state.component.ts
 * Qué hace:   Estado vacío.
 * Recibe:     @Input icon, text, image? (velorioMuestraN.webp), actionLabel?
 * Procesa:    —
 * Devuelve:   Ícono tenue + una línea + acción; @Output action.
 * Se conecta: ← listas y tableros
 * Estado:     Completo
 */

import { Component, EventEmitter, Input, Output } from '@angular/core';

@Component({
  selector: 'app-empty-state',
  standalone: true,
  template: `
    <div class="empty">
      @if (image) { <img [src]="image" alt="" width="160" /> } @else { <span class="ms">{{ icon }}</span> }
      <p>{{ text }}</p>
      @if (actionLabel) { <button class="btn" type="button" (click)="action.emit()">{{ actionLabel }}</button> }
    </div>`,
  styles: [`.empty { display: flex; flex-direction: column; align-items: center; gap: 10px; padding: 40px 16px; color: var(--text-muted); text-align: center; }
    .empty .ms { font-size: 40px; opacity: 0.5; } .empty img { opacity: 0.8; border-radius: var(--radius); } p { margin: 0; }`],
})
export class EmptyStateComponent {
  @Input() icon = 'inbox';
  @Input() text = 'No hay datos para mostrar.';
  @Input() image?: string;
  @Input() actionLabel?: string;
  @Output() action = new EventEmitter<void>();
}
