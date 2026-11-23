/**
 * Archivo:    frontend/src/app/core/http/demo-headers.interceptor.ts
 * Qué hace:   Agrega X-Demo-Role y X-Demo-Branch a cada petición /api.
 * Recibe:     HttpRequest saliente.
 * Procesa:    Lee RoleContextService y BranchContextService y clona la petición con los encabezados.
 * Devuelve:   HttpRequest modificada.
 * Se conecta: ← app.config.ts · → backend DemoUserMiddleware
 * Reglas:     RF-30
 * Estado:     Completo
 */

import { HttpInterceptorFn } from '@angular/common/http';
import { inject } from '@angular/core';
import { BranchContextService } from '../state/branch-context.service';
import { RoleContextService } from '../state/role-context.service';

export const demoHeaders: HttpInterceptorFn = (req, next) => {
  if (!req.url.startsWith('/api')) return next(req);
  const role = inject(RoleContextService);
  const branch = inject(BranchContextService);
  let headers = req.headers.set('X-Demo-Role', role.role());
  const b = branch.branchId();
  if (b !== null) headers = headers.set('X-Demo-Branch', String(b));
  const p = role.providerId();
  if (role.role() === 'Provider' && p !== null) headers = headers.set('X-Demo-Provider', String(p));
  return next(req.clone({ headers }));
};
