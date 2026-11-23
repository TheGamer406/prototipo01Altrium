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
 * Estado:     Parcial: pantallas provisionales; los módulos reales se enlazan en las fases 4 a 6
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
  page('inicio', 'Inicio', 'Resumen de la sede: servicios de hoy, alertas, capillas y cofres.', ['RF-19', 'RF-29'], ALL),
  { path: 'servicios', canActivate: [roleGuard], data: { roles: ['Advisor', 'BranchManager', 'Management'] }, loadChildren: () => import('./features/services/services.routes').then(m => m.servicesRoutes) },
  { path: 'proveedores', canActivate: [roleGuard], data: { roles: ['BranchManager', 'Management'] }, loadChildren: () => import('./features/providers/providers.routes').then(m => m.providersRoutes) },
  { path: 'bandeja', canActivate: [roleGuard], data: { roles: ['Provider'] }, loadComponent: () => import('./features/provider-inbox/provider-inbox.page').then(m => m.ProviderInboxPage) },
  page('capillas', 'Capillas', 'Calendario de capillas por sede.', ['RF-20'], ['Advisor', 'BranchManager', 'Management']),
  page('personal', 'Personal', 'Asignación de personal por turno.', ['RF-25'], ['BranchManager', 'Management']),
  page('traslados', 'Traslados', 'Mapa y hitos de los traslados.', ['RF-22', 'RF-23', 'RF-24'], ['BranchManager', 'Management']),
  page('cremacion', 'Cremación', 'Cola del crematorio central.', ['RF-21'], ['CrematoriumOperator', 'Management']),
  page('chofer', 'App del chofer', 'Hitos del traslado, incluso sin señal.', ['RF-22', 'RNF-05'], ['Driver']),
  page('inventario', 'Inventario de cofres', 'Existencias, alertas y reabastecimiento.', ['RF-26', 'RF-27', 'RF-28'], ['BranchManager', 'Management']),
  page('central', 'Vista central', 'Mapa de sedes, servicios en curso y alertas.', ['RF-29'], ['Management']),
  page('analitica', 'Analítica', 'Ventas, eficiencia por etapa y servicios por zona.', ['RF-32', 'RF-33', 'RF-34'], ['Management']),
  page('bitacora', 'Bitácora', 'Quién cambió qué y cuándo.', ['RF-31'], ['Management']),
  page('mensajes', 'Mensajes', 'Correos simulados enviados.', ['RF-35'], ['Management', 'BranchManager']),
  page('catalogo', 'Catálogo', 'Componentes, paquetes y modelos de cofre.', ['RF-04', 'RF-06'], ['Management']),
  page('parametros', 'Parámetros', 'Plazos y umbrales configurables.', ['RF-17', 'RF-24'], ['Management']),
  { path: '**', redirectTo: 'inicio' },
];
