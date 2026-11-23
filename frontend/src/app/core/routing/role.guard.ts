/**
 * Archivo:    frontend/src/app/core/routing/role.guard.ts
 * Qué hace:   Bloquea rutas que el rol actual no ve.
 * Recibe:     Ruta con data.roles.
 * Procesa:    RoleContextService.canSee.
 * Devuelve:   true o UrlTree al inicio del rol.
 * Se conecta: ← app.routes.ts
 * Reglas:     RF-30
 * Estado:     Completo
 */

import { inject } from '@angular/core';
import { CanActivateFn, Router } from '@angular/router';
import { ROLE_HOME } from '../navigation/nav-items';
import { RoleContextService } from '../state/role-context.service';
import { DemoRole } from '../models/services.models';

export const roleGuard: CanActivateFn = (route) => {
  const role = inject(RoleContextService);
  const router = inject(Router);
  const allowed = route.data?.['roles'] as DemoRole[] | undefined;
  return !allowed || allowed.includes(role.role()) ? true : router.parseUrl(ROLE_HOME[role.role()]);
};
