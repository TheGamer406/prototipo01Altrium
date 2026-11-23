/**
 * Archivo:    frontend/src/app/features/services/services.routes.ts
 * Qué hace:   Rutas del módulo Servicios.
 * Recibe:     —
 * Procesa:    '' → lista · nuevo → asistente (canDeactivate) · :id → detalle (reuseKey service-detail); las pestañas internas son componentes del detalle.
 * Devuelve:   Routes.
 * Se conecta: ← app.routes.ts
 * Estado:     Completo
 */

import { Routes } from '@angular/router';
import { unsavedChangesGuard } from '../../core/forms/unsaved-changes.guard';

export const servicesRoutes: Routes = [
  { path: '', loadComponent: () => import('./service-list.page').then(m => m.ServiceListPage) },
  { path: 'nuevo', canDeactivate: [unsavedChangesGuard], loadComponent: () => import('./new-service/new-service-wizard.component').then(m => m.NewServiceWizardComponent) },
  { path: ':id', data: { reuseKey: 'service-detail' }, loadComponent: () => import('./service-detail/service-detail.page').then(m => m.ServiceDetailPage) },
];
