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
 * Estado:     Parcial: generado por Angular CLI; rutas en fase 3
 */

import { Routes } from '@angular/router';

export const routes: Routes = [];
