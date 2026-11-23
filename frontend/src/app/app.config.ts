/**
 * Archivo:    frontend/src/app/app.config.ts
 * Qué hace:   Configuración global de la aplicación (providers standalone).
 * Recibe:     —
 * Procesa:    provideRouter(routes, withComponentInputBinding());
 *               provideHttpClient(withInterceptors([demoHeaders, errors])); provideAnimationsAsync;
 *               locale es-CR; RouteReuseStrategy = ErpRouteReuseStrategy; ngx-echarts.
 * Devuelve:   ApplicationConfig para main.ts.
 * Se conecta: ← main.ts · → app.routes.ts, core/http/*, core/routing/*
 * Estado:     Parcial: generado por Angular CLI; providers en fase 3
 */

import { ApplicationConfig, provideZoneChangeDetection } from '@angular/core';
import { provideRouter } from '@angular/router';

import { routes } from './app.routes';

export const appConfig: ApplicationConfig = {
  providers: [provideZoneChangeDetection({ eventCoalescing: true }), provideRouter(routes)]
};
