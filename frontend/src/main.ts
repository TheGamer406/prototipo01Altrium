/**
 * Archivo:    frontend/src/main.ts
 * Qué hace:   Arranca la aplicación Angular.
 * Recibe:     —
 * Procesa:    bootstrapApplication(AppComponent, appConfig).
 * Devuelve:   La app montada en <app-root> de index.html.
 * Se conecta: → app/app.component.ts, app/app.config.ts
 * Estado:     Completo
 */
import { bootstrapApplication } from '@angular/platform-browser';
import { appConfig } from './app/app.config';
import { AppComponent } from './app/app.component';

bootstrapApplication(AppComponent, appConfig)
  .catch((err) => console.error(err));
