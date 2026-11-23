/**
 * Archivo:    frontend/src/app/shared/components/rf-tag.component.ts
 * Qué hace:   Etiqueta discreta con los RF de la pantalla.
 * Recibe:     @Input rf: string[].
 * Procesa:    —
 * Devuelve:   'RF-19 · RF-25'.
 * Se conecta: ← page-header
 * Estado:     Completo
 */

import { Component, Input } from '@angular/core';

@Component({
  selector: 'app-rf-tag',
  standalone: true,
  template: `@if (rf.length) { <span class="rf" [title]="'Requerimientos que cubre esta pantalla'">{{ rf.join(' · ') }}</span> }`,
  styles: [`.rf { font-size: 11px; color: var(--text-muted); border: 1px solid var(--border); border-radius: 4px; padding: 1px 6px; letter-spacing: 0.3px; }`],
})
export class RfTagComponent { @Input() rf: string[] = []; }
