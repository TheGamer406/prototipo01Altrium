/**
 * Archivo:    frontend/src/app/core/layout/footer.component.ts
 * Qué hace:   Pie mínimo.
 * Recibe:     —
 * Procesa:    —
 * Devuelve:   'Acompaña · Prototipo para Funeraria Los Cipreses · ATRIUM Solutions'.
 * Se conecta: ← shell
 * Estado:     Completo
 */

import { Component } from '@angular/core';

@Component({
  selector: 'app-footer',
  standalone: true,
  template: `<footer>Acompaña · Prototipo para Funeraria Los Cipreses · ATRIUM Solutions</footer>`,
  styles: [`footer { padding: 10px 24px; font-size: 12px; color: var(--text-muted); border-top: 1px solid var(--border); }`],
})
export class FooterComponent {}
