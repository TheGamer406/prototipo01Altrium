/**
 * Archivo:    frontend/src/app/core/navigation/nav-items.ts
 * Qué hace:   Definición del menú lateral por sección y rol.
 * Recibe:     —
 * Procesa:    Constante NAV: secciones Operación, Logística, Inventario, Gerencia, Configuración con
 *               ítems {label, icon (Material Symbols), route, roles[], rf}.
 * Devuelve:   NavSection[] filtrado por rol en sidebar.
 * Se conecta: ← sidebar.component.ts, role.guard.ts
 * Reglas:     RF-30
 * Estado:     Completo
 */

import { DemoRole } from '../models/services.models';

export interface NavItem { label: string; icon: string; route: string; roles: DemoRole[]; rf: string[]; }
export interface NavSection { title: string; items: NavItem[]; }

const ALL: DemoRole[] = ['Advisor', 'BranchManager', 'CrematoriumOperator', 'Provider', 'Management', 'Driver'];

export const NAV: NavSection[] = [
  {
    title: 'Operación',
    items: [
      { label: 'Inicio', icon: 'home', route: '/inicio', roles: ALL, rf: ['RF-19'] },
      { label: 'Servicios', icon: 'assignment', route: '/servicios', roles: ['Advisor', 'BranchManager', 'Management'], rf: ['RF-01', 'RF-19'] },
      { label: 'Proveedores', icon: 'storefront', route: '/proveedores', roles: ['BranchManager', 'Management'], rf: ['RF-14', 'RF-17'] },
      { label: 'Bandeja del proveedor', icon: 'inbox', route: '/bandeja', roles: ['Provider'], rf: ['RF-15', 'RF-16'] },
    ],
  },
  {
    title: 'Logística',
    items: [
      { label: 'Capillas', icon: 'church', route: '/capillas', roles: ['Advisor', 'BranchManager', 'Management'], rf: ['RF-20'] },
      { label: 'Personal', icon: 'groups', route: '/personal', roles: ['BranchManager', 'Management'], rf: ['RF-25'] },
      { label: 'Traslados', icon: 'local_shipping', route: '/traslados', roles: ['BranchManager', 'Management'], rf: ['RF-22', 'RF-23', 'RF-24'] },
      { label: 'Cremación', icon: 'local_fire_department', route: '/cremacion', roles: ['CrematoriumOperator', 'Management'], rf: ['RF-21'] },
      { label: 'App del chofer', icon: 'smartphone', route: '/chofer', roles: ['Driver'], rf: ['RF-22', 'RNF-05'] },
    ],
  },
  {
    title: 'Inventario',
    items: [{ label: 'Inventario de cofres', icon: 'inventory_2', route: '/inventario', roles: ['BranchManager', 'Management'], rf: ['RF-26', 'RF-27', 'RF-28'] }],
  },
  {
    title: 'Gerencia',
    items: [
      { label: 'Vista central', icon: 'map', route: '/central', roles: ['Management'], rf: ['RF-29'] },
      { label: 'Analítica', icon: 'monitoring', route: '/analitica', roles: ['Management'], rf: ['RF-32', 'RF-33', 'RF-34'] },
      { label: 'Bitácora', icon: 'history', route: '/bitacora', roles: ['Management'], rf: ['RF-31'] },
      { label: 'Mensajes', icon: 'mail', route: '/mensajes', roles: ['Management', 'BranchManager'], rf: ['RF-35'] },
    ],
  },
  {
    title: 'Configuración',
    items: [
      { label: 'Catálogo', icon: 'category', route: '/catalogo', roles: ['Management'], rf: ['RF-04', 'RF-06'] },
      { label: 'Parámetros', icon: 'tune', route: '/parametros', roles: ['Management'], rf: ['RF-17', 'RF-24'] },
    ],
  },
];

/** Pantalla de inicio por rol (a donde se navega si el rol no ve la ruta actual). */
export const ROLE_HOME: Record<DemoRole, string> = {
  Advisor: '/servicios', BranchManager: '/servicios', CrematoriumOperator: '/cremacion',
  Provider: '/bandeja', Management: '/central', Driver: '/chofer',
};
