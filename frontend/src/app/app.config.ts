/**
 * Archivo:    frontend/src/app/app.config.ts
 * Qué hace:   Configuración global de la aplicación (providers standalone).
 * Recibe:     —
 * Procesa:    provideRouter(routes, withComponentInputBinding());
 *               provideHttpClient(withInterceptors([demoHeaders, errors])); provideAnimationsAsync;
 *               locale es-CR; RouteReuseStrategy = ErpRouteReuseStrategy; ngx-echarts.
 * Devuelve:   ApplicationConfig para main.ts.
 * Se conecta: ← main.ts · → app.routes.ts, core/http/*, core/routing/*
 * Estado:     Completo
 */

import { registerLocaleData } from '@angular/common';
import localeEsCr from '@angular/common/locales/es-CR';
import { provideHttpClient, withInterceptors } from '@angular/common/http';
import { ApplicationConfig, LOCALE_ID, provideZoneChangeDetection } from '@angular/core';
import { MAT_SNACK_BAR_DEFAULT_OPTIONS } from '@angular/material/snack-bar';
import { provideAnimationsAsync } from '@angular/platform-browser/animations/async';
import { RouteReuseStrategy, provideRouter, withComponentInputBinding } from '@angular/router';
import { provideEchartsCore } from 'ngx-echarts';
import * as echarts from 'echarts/core';
import { BarChart, LineChart, PieChart, HeatmapChart } from 'echarts/charts';
import { GridComponent, LegendComponent, TitleComponent, TooltipComponent, VisualMapComponent } from 'echarts/components';
import { CanvasRenderer } from 'echarts/renderers';
import { routes } from './app.routes';
import { demoHeaders } from './core/http/demo-headers.interceptor';
import { errors } from './core/http/error.interceptor';
import { ErpRouteReuseStrategy } from './core/routing/erp-route-reuse.strategy';
import { CHART_THEME_NAME, chartTheme } from './shared/charts/chart-theme';

registerLocaleData(localeEsCr);
echarts.use([BarChart, LineChart, PieChart, HeatmapChart, GridComponent, LegendComponent, TitleComponent, TooltipComponent, VisualMapComponent, CanvasRenderer]);
echarts.registerTheme(CHART_THEME_NAME, chartTheme);

export const appConfig: ApplicationConfig = {
  providers: [
    provideZoneChangeDetection({ eventCoalescing: true }),
    provideRouter(routes, withComponentInputBinding()),
    provideHttpClient(withInterceptors([demoHeaders, errors])),
    provideAnimationsAsync(),
    provideEchartsCore({ echarts }),
    { provide: LOCALE_ID, useValue: 'es-CR' },
    { provide: RouteReuseStrategy, useExisting: ErpRouteReuseStrategy },
    { provide: MAT_SNACK_BAR_DEFAULT_OPTIONS, useValue: { horizontalPosition: 'right', verticalPosition: 'bottom' } },
  ],
};
