/**
 * Archivo:    frontend/src/app/shared/components/logo.component.ts
 * Qué hace:   Logo SVG: flor de trazo simple + 'Acompaña'.
 * Recibe:     @Input compact (solo flor).
 * Procesa:    —
 * Devuelve:   SVG inline en currentColor.
 * Se conecta: ← topbar, favicon
 * Estado:     Completo
 */

import { Component, Input } from '@angular/core';

@Component({
  selector: 'app-logo',
  standalone: true,
  template: `
    <span class="logo" aria-label="Acompaña">
      <svg viewBox="0 0 32 32" width="26" height="26" fill="none" stroke="currentColor" stroke-width="1.6" stroke-linecap="round" aria-hidden="true">
        <path d="M16 28V15"/><path d="M16 15c-4-1-6-4-6-8 4 0 6 3 6 8z"/><path d="M16 15c4-1 6-4 6-8-4 0-6 3-6 8z"/>
        <path d="M16 20c-3-.5-5-2.5-5-5.5"/><path d="M16 20c3-.5 5-2.5 5-5.5"/>
      </svg>
      @if (!compact) { <span class="name">Acompaña</span> }
    </span>`,
  styles: [`.logo { display: inline-flex; align-items: center; gap: 8px; color: var(--accent-text); } .name { font-weight: 600; font-size: 16px; color: var(--text); letter-spacing: 0.2px; }`],
})
export class LogoComponent { @Input() compact = false; }
