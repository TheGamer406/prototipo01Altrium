/**
 * Archivo:    frontend/src/app/core/state/layout.service.ts
 * Qué hace:   Estado visual compartido del layout (barra lateral plegada).
 * Recibe:     toggle() desde el botón de la barra lateral.
 * Procesa:    collapsed = signal<boolean>; persiste en localStorage.
 * Devuelve:   collapsed(); el shell ajusta el ancho de la barra (248 → 64 px).
 * Se conecta: ← sidebar.component, shell.component
 * Estado:     Completo
 */
import { Injectable, signal } from '@angular/core';

@Injectable({ providedIn: 'root' })
export class LayoutService {
  readonly collapsed = signal<boolean>(this.read());
  private read(): boolean { try { return localStorage.getItem('acompana.sidebar') === '1'; } catch { return false; } }
  toggle(): void {
    this.collapsed.update(v => !v);
    try { localStorage.setItem('acompana.sidebar', this.collapsed() ? '1' : '0'); } catch { /* sin almacenamiento */ }
  }
}
