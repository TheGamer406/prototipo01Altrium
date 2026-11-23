/**
 * Archivo:    frontend/src/app/app.component.ts
 * Qué hace:   Componente raíz: solo monta el ShellComponent.
 * Recibe:     —
 * Procesa:    —
 * Devuelve:   <app-shell/>
 * Se conecta: ← main.ts · → core/layout/shell.component.ts
 * Estado:     Completo
 */

import { Component } from '@angular/core';
import { ShellComponent } from './core/layout/shell.component';

@Component({
  selector: 'app-root',
  standalone: true,
  imports: [ShellComponent],
  template: '<app-shell />',
})
export class AppComponent {}
