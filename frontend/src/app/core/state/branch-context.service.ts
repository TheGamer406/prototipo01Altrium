/**
 * Archivo:    frontend/src/app/core/state/branch-context.service.ts
 * Qué hace:   Sede actual (signal).
 * Recibe:     Selección en la barra superior; lista de sedes de GET /api/meta/branches.
 * Procesa:    branchId = signal<number|null>; null solo permitido para Gerencia ('Todas').
 * Devuelve:   branchId(), branchName(), branches().
 * Se conecta: ← topbar · → demo-headers.interceptor.ts, ops-hub.service.ts
 * Reglas:     RF-30
 * Estado:     Completo
 */

import { Injectable, computed, inject, signal } from '@angular/core';
import { MetaApi } from '../api/meta.api';
import { Branch } from '../models/meta.models';

@Injectable({ providedIn: 'root' })
export class BranchContextService {
  private meta = inject(MetaApi);
  readonly branches = signal<Branch[]>([]);
  readonly branchId = signal<number | null>(this.stored());
  readonly branchName = computed(() => this.branches().find(b => b.id === this.branchId())?.name ?? 'Todas');

  constructor() {
    this.meta.branches().subscribe(list => {
      this.branches.set(list);
      if (this.branchId() === null && localStorage.getItem('acompana.branch') !== 'all') {
        this.set(list.find(b => b.name === 'Heredia')?.id ?? list[0]?.id ?? null);
      }
    });
  }

  private stored(): number | null {
    try { const v = localStorage.getItem('acompana.branch'); return v && v !== 'all' ? Number(v) : null; } catch { return null; }
  }

  set(id: number | null): void {
    this.branchId.set(id);
    try { localStorage.setItem('acompana.branch', id === null ? 'all' : String(id)); } catch { /* sin almacenamiento */ }
  }

  /** Gerencia puede ver 'Todas'; los demás roles necesitan una sede. */
  ensureFor(role: string): void {
    if (role !== 'Management' && this.branchId() === null) this.set(this.branches().find(b => b.name === 'Heredia')?.id ?? this.branches()[0]?.id ?? null);
  }
}
