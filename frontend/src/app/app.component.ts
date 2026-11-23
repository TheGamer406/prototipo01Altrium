/**
 * Archivo:    frontend/src/app/app.component.ts
 * Qué hace:   Componente raíz: solo monta el ShellComponent.
 * Recibe:     —
 * Procesa:    —
 * Devuelve:   <app-shell/>
 * Se conecta: ← main.ts · → core/layout/shell.component.ts
 * Estado:     Parcial: generado por Angular CLI; shell en fase 3
 */

import { Component } from '@angular/core';
import { RouterOutlet } from '@angular/router';

@Component({
  selector: 'app-root',
  imports: [RouterOutlet],
  template: '<router-outlet />',
})
export class AppComponent {
  title = 'frontend';
}
