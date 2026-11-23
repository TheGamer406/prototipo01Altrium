/**
 * Archivo:    frontend/src/app/features/services/new-service/step-family.component.ts
 * Qué hace:   Paso 2: familia responsable y consentimiento.
 * Recibe:     FormGroup 'family'.
 * Procesa:    Correo obligatorio (la familia recibe avisos por correo); casilla de consentimiento con
 *               texto de la Ley 8968.
 * Devuelve:   Cambios en el FormGroup del asistente (no llama al API por su cuenta).
 * Se conecta: ← new-service-wizard · → CatalogApi / LogisticsApi (solo lectura)
 * Reglas:     RF-35 · RF-36 · RNF-14
 * Estado:     Completo
 */

import { Component, Input } from '@angular/core';
import { FormGroup, ReactiveFormsModule } from '@angular/forms';

@Component({
  selector: 'app-step-family',
  standalone: true,
  imports: [ReactiveFormsModule],
  template: `
    <div [formGroup]="form" class="grid"><div formGroupName="family" class="grid full">
      <label class="full">Nombre del responsable<input formControlName="fullName" autocomplete="off" /><small class="err">{{ err('fullName') }}</small></label>
      <label>Cédula<input formControlName="nationalId" placeholder="1-0555-0123" /><small class="err">{{ err('nationalId') }}</small></label>
      <label>Parentesco<select formControlName="relationship">@for (r of rels; track r) { <option [value]="r">{{ r }}</option> }</select></label>
      <label>Teléfono<input formControlName="phone" placeholder="8888-1234" /><small class="err">{{ err('phone') }}</small></label>
      <label>Correo (recibe los avisos)<input type="email" formControlName="email" /><small class="err">{{ err('email') }}</small></label>
      <label class="full check"><input type="checkbox" formControlName="insuranceProspectConsent" />
        <span>La familia autoriza que Funeraria Los Cipreses use estos datos para ofrecerle seguros funerarios (Ley 8968 de protección de datos personales). Es opcional.</span></label>
    </div></div>`,
  styles: [`.grid { display: grid; grid-template-columns: 1fr 1fr; gap: 12px 16px; } .full { grid-column: 1 / -1; } label { display: flex; flex-direction: column; gap: 4px; font-size: 12px; color: var(--text-muted); }
    .check { flex-direction: row; align-items: flex-start; color: var(--text); font-size: 13px; } .check input { height: auto; margin-top: 3px; } .err { color: #e0a0a0; min-height: 14px; }
    @media (max-width: 768px) { .grid { grid-template-columns: 1fr; } }`],
})
export class StepFamilyComponent {
  @Input({ required: true }) form!: FormGroup;
  rels = ['Hijo', 'Hija', 'Esposo', 'Esposa', 'Hermano', 'Hermana', 'Nieto', 'Nieta', 'Sobrino', 'Sobrina', 'Otro'];
  err(c: string): string {
    const k = this.form.get(['family', c]);
    if (!k || !(k.touched || k.dirty) || !k.errors) return '';
    return k.errors['server'] ?? (k.errors['required'] ? 'Campo obligatorio.' : k.errors['email'] ? 'Ingrese un correo válido.' : 'Formato no válido.');
  }
}
