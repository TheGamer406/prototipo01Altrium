/**
 * Archivo:    frontend/src/app/features/settings/settings.page.ts
 * Qué hace:   Parámetros configurables.
 * Recibe:     —
 * Procesa:    Plazo de aceptación (min), umbral de traslado (min), mínimo por defecto.
 * Devuelve:   Formulario.
 * Se conecta: → OversightApi
 * Reglas:     RF-17 · RF-24
 * Estado:     Completo
 */

import { Component, inject, signal } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { MatSnackBar } from '@angular/material/snack-bar';
import { OversightApi } from '../../core/api/oversight.api';
import { PageHeaderComponent } from '../../core/layout/page-header.component';

const FIELDS = [
  ['ProviderAcceptTimeoutMin', 'Plazo para que un proveedor acepte (minutos)', 'Pasado este plazo se vencen las ofertas y se alerta a la sede.'],
  ['TransferThresholdMin', 'Demora máxima entre hitos del traslado (minutos)', 'Si un tramo supera el umbral se genera una alerta.'],
  ['DefaultMinStock', 'Mínimo de cofres por modelo y sede', 'Al llegar a este nivel se alerta y se pide reabastecimiento.'],
] as const;

@Component({
  selector: 'app-settings-page',
  standalone: true,
  imports: [FormsModule, PageHeaderComponent],
  template: `
    <app-page-header title="Parámetros" description="Valores que Gerencia puede cambiar sin programar." [rf]="['RF-17','RF-24']" />
    <form class="card" (ngSubmit)="save()">@for (f of fields; track f[0]) {
      <label><span>{{ f[1] }}</span><input type="number" min="1" [name]="f[0]" [(ngModel)]="values[f[0]]" /><small class="muted">{{ f[2] }}</small></label> }
      <button class="btn primary" type="submit">Guardar</button></form>`,
  styles: [`form { display: flex; flex-direction: column; gap: 14px; max-width: 520px; } label { display: flex; flex-direction: column; gap: 4px; } input { width: 140px; } button { align-self: flex-start; }`],
})
export class SettingsPage {
  private api = inject(OversightApi);
  private snack = inject(MatSnackBar);
  readonly fields = FIELDS;
  values: Record<string, string> = {};
  readonly loaded = signal(false);
  constructor() { this.api.settings().subscribe(s => { this.values = { ...s }; this.loaded.set(true); }); }
  save(): void { this.api.saveSettings(this.values).subscribe(s => { this.values = { ...s }; this.snack.open('Parámetros guardados.', 'Cerrar', { duration: 3000, panelClass: 'success' }); }); }
}
