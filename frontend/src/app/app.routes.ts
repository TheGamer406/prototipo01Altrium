/**
 * Archivo:    frontend/src/app/app.routes.ts
 * Qué hace:   Rutas de primer nivel; cada módulo se carga perezoso.
 * Recibe:     —
 * Procesa:    '' → home; 'servicios' → services.routes; 'proveedores'; 'bandeja'; 'capillas';
 *               'personal'; 'cremacion'; 'traslados'; 'inventario'; 'central'; 'analitica'; 'bitacora';
 *               'catalogo'; 'mensajes'; 'parametros'; 'chofer'. Guardas por rol (roleGuard).
 * Devuelve:   Routes.
 * Se conecta: ← app.config.ts · → features/<módulo>/<módulo>.routes.ts
 * Reglas:     RF-30
 * Estado:     Completo
 */

import { inject } from '@angular/core';
import { Router, Routes } from '@angular/router';
import { ROLE_HOME } from './core/navigation/nav-items';
import { roleGuard } from './core/routing/role.guard';
import { RoleContextService } from './core/state/role-context.service';
import { PlaceholderPage } from './shared/components/placeholder.page';
import { DemoRole } from './core/models/services.models';

const ALL: DemoRole[] = ['Advisor', 'BranchManager', 'CrematoriumOperator', 'Provider', 'Management', 'Driver'];

const page = (path: string, title: string, description: string, rf: string[], roles: DemoRole[]) => ({
  path, component: PlaceholderPage, canActivate: [roleGuard], data: { title, description, rf, roles },
});

export const routes: Routes = [
  { path: '', pathMatch: 'full', canActivate: [() => inject(Router).parseUrl(ROLE_HOME[inject(RoleContextService).role()])], children: [] },
  { path: 'inicio', canActivate: [roleGuard], data: { roles: ALL }, loadComponent: () => import('./features/home/home.page').then(m => m.HomePage) },
  { path: 'servicios', canActivate: [roleGuard], data: { roles: ['Advisor', 'BranchManager', 'Management'] }, loadChildren: () => import('./features/services/services.routes').then(m => m.servicesRoutes) },
  { path: 'proveedores', canActivate: [roleGuard], data: { roles: ['BranchManager', 'Management'] }, loadChildren: () => import('./features/providers/providers.routes').then(m => m.providersRoutes) },
  { path: 'bandeja', canActivate: [roleGuard], data: { roles: ['Provider'] }, loadComponent: () => import('./features/provider-inbox/provider-inbox.page').then(m => m.ProviderInboxPage) },
  { path: 'capillas', canActivate: [roleGuard], data: { roles: ['Advisor', 'BranchManager', 'Management'] }, loadComponent: () => import('./features/chapels/chapel-calendar.page').then(m => m.ChapelCalendarPage) },
  { path: 'personal', canActivate: [roleGuard], data: { roles: ['BranchManager', 'Management'] }, loadComponent: () => import('./features/staff/staff-assignments.page').then(m => m.StaffAssignmentsPage) },
  { path: 'traslados', canActivate: [roleGuard], data: { roles: ['BranchManager', 'Management'] }, children: [
    { path: '', loadComponent: () => import('./features/transfers/transfers-map.page').then(m => m.TransfersMapPage) },
    { path: ':id', loadComponent: () => import('./features/transfers/transfer-milestones.page').then(m => m.TransferMilestonesPage) } ] },
  { path: 'cremacion', canActivate: [roleGuard], data: { roles: ['CrematoriumOperator', 'Management'] }, loadComponent: () => import('./features/cremation/cremation-board.page').then(m => m.CremationBoardPage) },
  { path: 'chofer', canActivate: [roleGuard], data: { roles: ['Driver'] }, loadComponent: () => import('./features/driver-app/driver-app.page').then(m => m.DriverAppPage) },
  { path: 'inventario', canActivate: [roleGuard], data: { roles: ['BranchManager', 'Management'] }, loadChildren: () => import('./features/inventory/inventory.routes').then(m => m.inventoryRoutes) },
  { path: 'central', canActivate: [roleGuard], data: { roles: ['Management'] }, loadChildren: () => import('./features/central/central.routes').then(m => m.centralRoutes) },
  { path: 'analitica', canActivate: [roleGuard], data: { roles: ['Management'] }, loadChildren: () => import('./features/analytics/analytics.routes').then(m => m.analyticsRoutes) },
  { path: 'bitacora', canActivate: [roleGuard], data: { roles: ['Management'] }, loadComponent: () => import('./features/audit/audit-log.page').then(m => m.AuditLogPage) },
  { path: 'mensajes', canActivate: [roleGuard], data: { roles: ['Management', 'BranchManager'] }, loadComponent: () => import('./features/messages/sent-messages.page').then(m => m.SentMessagesPage) },
  { path: 'catalogo', canActivate: [roleGuard], data: { roles: ['Management'] }, loadChildren: () => import('./features/catalog/catalog.routes').then(m => m.catalogRoutes) },
  { path: 'parametros', canActivate: [roleGuard], data: { roles: ['Management'] }, loadComponent: () => import('./features/settings/settings.page').then(m => m.SettingsPage) },
  { path: '**', redirectTo: 'inicio' },
];
