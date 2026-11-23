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
  private tablet = typeof matchMedia !== 'undefined' ? matchMedia('(max-width: 1024px)') : null;
  private phone = typeof matchMedia !== 'undefined' ? matchMedia('(max-width: 768px)') : null;
  readonly collapsed = signal<boolean>(this.read());
  /** En móvil (≤ 768 px) el menú es un panel que se abre con el botón de la barra superior. */
  readonly isMobile = signal<boolean>(this.phone?.matches ?? false);
  readonly mobileOpen = signal(false);

  constructor() {
    this.phone?.addEventListener('change', e => { this.isMobile.set(e.matches); this.mobileOpen.set(false); });
    this.tablet?.addEventListener('change', e => this.collapsed.set(e.matches || this.stored() === '1'));
  }

  private stored(): string | null { try { return localStorage.getItem('acompana.sidebar'); } catch { return null; } }
  // tablet (≤ 1024 px): arranca plegado salvo que la persona lo haya expandido
  private read(): boolean { const v = this.stored(); return v === null ? (this.tablet?.matches ?? false) : v === '1'; }
  toggleMobile(): void { this.mobileOpen.update(v => !v); }
  toggle(): void {
    this.collapsed.update(v => !v);
    try { localStorage.setItem('acompana.sidebar', this.collapsed() ? '1' : '0'); } catch { /* sin almacenamiento */ }
  }
}
