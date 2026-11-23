/**
 * Archivo:    frontend/src/app/core/state/open-tabs.service.ts
 * Qué hace:   Pestañas ERP de servicios abiertos.
 * Recibe:     open(id, code) desde la lista o la búsqueda; close(id).
 * Procesa:    tabs = signal<{id, code, dirty}[]>; máximo 8; persiste en sessionStorage.
 * Devuelve:   tabs(), active(); erp-tabs-bar los muestra.
 * Se conecta: ← service-list, topbar (búsqueda) · → erp-tabs-bar, ErpRouteReuseStrategy (libera la
 *               ruta al cerrar)
 * Estado:     Completo
 */

import { Injectable, inject, signal } from '@angular/core';
import { Router } from '@angular/router';
import { ErpRouteReuseStrategy } from '../routing/erp-route-reuse.strategy';

export interface ErpTab { id: string; code: string; dirty: boolean; }
const MAX_TABS = 8;

@Injectable({ providedIn: 'root' })
export class OpenTabsService {
  private router = inject(Router);
  private reuse = inject(ErpRouteReuseStrategy);
  readonly tabs = signal<ErpTab[]>(this.restore());

  private restore(): ErpTab[] {
    try { return JSON.parse(sessionStorage.getItem('acompana.tabs') ?? '[]'); } catch { return []; }
  }
  private save(): void { try { sessionStorage.setItem('acompana.tabs', JSON.stringify(this.tabs())); } catch { /* sin almacenamiento */ } }

  /** Abre (o enfoca) el servicio como pestaña ERP. */
  open(id: string, code: string): void {
    if (!this.tabs().some(t => t.id === id)) {
      const next = [...this.tabs(), { id, code, dirty: false }];
      if (next.length > MAX_TABS) { const dropped = next.shift()!; this.reuse.release(dropped.id); }
      this.tabs.set(next); this.save();
    }
    void this.router.navigate(['/servicios', id]);
  }

  setDirty(id: string, dirty: boolean): void {
    this.tabs.update(t => t.map(x => x.id === id ? { ...x, dirty } : x)); this.save();
  }

  close(id: string): void {
    const remaining = this.tabs().filter(t => t.id !== id);
    this.tabs.set(remaining); this.save();
    this.reuse.release(id);
    if (this.router.url.includes(id)) void this.router.navigateByUrl(remaining.length ? `/servicios/${remaining[remaining.length - 1].id}` : '/servicios');
  }
}
