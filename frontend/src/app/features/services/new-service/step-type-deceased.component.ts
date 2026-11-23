/**
 * Archivo:    frontend/src/app/features/services/new-service/step-type-deceased.component.ts
 * Qué hace:   Paso 1: tipo (inmediato/anticipado) y datos del fallecido.
 * Recibe:     FormGroup 'deceased' del asistente.
 * Procesa:    Valida cédula 1-0234-0567; fecha de defunción obligatoria si es inmediato.
 * Devuelve:   Cambios en el FormGroup del asistente (no llama al API por su cuenta).
 * Se conecta: ← new-service-wizard · → CatalogApi / LogisticsApi (solo lectura)
 * Reglas:     RF-01 · RF-02
 * Estado:     Completo
 */

import { Component, Input } from '@angular/core';
import { FormGroup, ReactiveFormsModule } from '@angular/forms';

@Component({
  selector: 'app-step-type-deceased',
  standalone: true,
  imports: [ReactiveFormsModule],
  template: `
    <div [formGroup]="form" class="grid">
      <fieldset class="full"><legend>Tipo de servicio</legend>
        <label><input type="radio" formControlName="kind" value="Immediate" /> Inmediato (ya ocurrió el fallecimiento)</label>
        <label><input type="radio" formControlName="kind" value="Advance" /> Anticipado (se contrata en vida)</label>
      </fieldset>
      <div formGroupName="deceased" class="grid full">
        <label class="full">Nombre completo<input formControlName="fullName" autocomplete="off" /><small class="err">{{ err('fullName') }}</small></label>
        <label>Cédula<input formControlName="nationalId" placeholder="1-0234-0567" /><small class="err">{{ err('nationalId') }}</small></label>
        <label>Fecha de nacimiento<input type="date" formControlName="birthDate" /><small class="err">{{ err('birthDate') }}</small></label>
        @if (form.get('kind')?.value === 'Immediate') {
          <label>Fecha de defunción<input type="date" formControlName="deathDate" /><small class="err">{{ err('deathDate') }}</small></label>
        }
      </div>
    </div>`,
  styles: [`.grid { display: grid; grid-template-columns: 1fr 1fr; gap: 12px 16px; } .full { grid-column: 1 / -1; } label { display: flex; flex-direction: column; gap: 4px; font-size: 12px; color: var(--text-muted); }
    fieldset { border: 1px solid var(--border); border-radius: var(--radius); display: flex; gap: 24px; } fieldset label { flex-direction: row; align-items: center; color: var(--text); font-size: 13px; } input[type=radio] { height: auto; }
    .err { color: #e0a0a0; min-height: 14px; }`],
})
export class StepTypeDeceasedComponent {
  @Input({ required: true }) form!: FormGroup;
  err(c: string): string {
    const k = this.form.get(['deceased', c]);
    if (!k || !(k.touched || k.dirty) || !k.errors) return '';
    return k.errors['server'] ?? (k.errors['required'] ? 'Campo obligatorio.' : k.errors['pattern'] ? 'Use el formato 1-0234-0567.' : 'Valor no válido.');
  }
}
