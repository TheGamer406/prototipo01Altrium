/**
 * Archivo:    frontend/src/app/core/state/admin-mode.service.ts
 * Qué hace:   Interruptor de modo admin del demo.
 * Recibe:     Toggle de la barra superior.
 * Procesa:    enabled = signal<boolean>; persiste en localStorage.
 * Devuelve:   enabled(); muestra banda ámbar y menú Admin.
 * Se conecta: ← topbar · → admin-menu.component.ts
 * Estado:     Completo
 */

import { Injectable, signal } from '@angular/core';

@Injectable({ providedIn: 'root' })
export class AdminModeService {
  readonly enabled = signal<boolean>(this.read());

  private read(): boolean { try { return localStorage.getItem('acompana.admin') === '1'; } catch { return false; } }

  toggle(): void {
    this.enabled.update(v => !v);
    try { localStorage.setItem('acompana.admin', this.enabled() ? '1' : '0'); } catch { /* sin almacenamiento */ }
  }
}
