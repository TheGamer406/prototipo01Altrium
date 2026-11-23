/**
 * Archivo:    frontend/src/app/core/state/role-context.service.ts
 * Qué hace:   Rol demo actual (signal) y permisos derivados.
 * Recibe:     Selección del usuario en la barra superior; valor inicial desde localStorage.
 * Procesa:    role = signal<DemoRole>('Advisor'); canSee(module) según la tabla de módulos por rol;
 *               persiste en localStorage.
 * Devuelve:   role(), roleLabel(), canSee(); lo leen menú, guardas e interceptor.
 * Se conecta: ← topbar · → nav-items.ts, demo-headers.interceptor.ts, roleGuard
 * Reglas:     RF-30
 * Estado:     Completo
 *
 * Contrato:
 *   setRole(role: DemoRole): void
 *     Recibe:     rol elegido
 *     Procesa:    actualiza la signal; si el rol no ve la ruta actual, navega a su inicio
 *     Devuelve:   void
 */

import { Injectable, computed, inject, signal } from '@angular/core';
import { Router } from '@angular/router';
import { NAV, ROLE_HOME } from '../navigation/nav-items';
import { DemoRole } from '../models/services.models';

const LABELS: Record<DemoRole, string> = {
  Advisor: 'Asesor funerario', BranchManager: 'Encargado de sede', CrematoriumOperator: 'Operador de crematorio',
  Provider: 'Proveedor', Management: 'Gerencia', Driver: 'Chofer',
};

@Injectable({ providedIn: 'root' })
export class RoleContextService {
  private router = inject(Router);
  readonly roles = Object.entries(LABELS).map(([value, label]) => ({ value: value as DemoRole, label }));
  readonly role = signal<DemoRole>(this.stored('role', 'Advisor') as DemoRole);
  readonly providerId = signal<number | null>(Number(this.stored('provider', '')) || null);
  readonly roleLabel = computed(() => LABELS[this.role()]);

  private stored(key: string, fallback: string): string {
    try { return localStorage.getItem(`acompana.${key}`) ?? fallback; } catch { return fallback; }
  }
  private persist(key: string, value: string): void {
    try { localStorage.setItem(`acompana.${key}`, value); } catch { /* sin almacenamiento */ }
  }

  setRole(role: DemoRole): void {
    this.role.set(role);
    this.persist('role', role);
    if (!this.canSee(this.router.url)) void this.router.navigateByUrl(ROLE_HOME[role]);
  }

  setProvider(id: number | null): void {
    this.providerId.set(id);
    this.persist('provider', id ? String(id) : '');
  }

  /** ¿El rol actual ve esta ruta? Se compara por el primer segmento. */
  canSee(url: string): boolean {
    const seg = '/' + (url.split('?')[0].split('/')[1] ?? '');
    if (seg === '/') return true;
    const item = NAV.flatMap(s => s.items).find(i => i.route === seg);
    return !item || item.roles.includes(this.role());
  }
}
