/**
 * Archivo:    frontend/src/app/features/services/new-service/step-payment-docs.component.ts
 * Qué hace:   Paso 6: pagador, acta y resumen.
 * Recibe:     FormGroup 'payment' + totales.
 * Procesa:    Familia | aseguradora (póliza); acta: número + archivo PDF/JPG/PNG; resumen con desglose
 *               de IVA y exoneraciones.
 * Devuelve:   Cambios en el FormGroup del asistente (no llama al API por su cuenta).
 * Se conecta: ← new-service-wizard · → CatalogApi / LogisticsApi (solo lectura)
 * Reglas:     RF-03 · RF-10 · RF-11
 * Estado:     Completo
 */

import { Component, EventEmitter, Input, Output } from '@angular/core';
import { FormGroup, ReactiveFormsModule } from '@angular/forms';
import { Option } from '../../../core/models/meta.models';
import { ColonesPipe } from '../../../shared/pipes/colones.pipe';
import { Totals } from './invoice-math';

@Component({
  selector: 'app-step-payment-docs',
  standalone: true,
  imports: [ReactiveFormsModule, ColonesPipe],
  template: `
    <div [formGroup]="form" class="grid"><div formGroupName="payment" class="grid full">
      <label class="full">¿Quién paga?<select formControlName="payer"><option value="Family">Familia</option><option value="Insurer">Aseguradora</option></select></label>
      @if (form.get('payment.payer')?.value === 'Insurer') {
        <label>Aseguradora<select formControlName="insurerId">@for (i of insurers; track i.value) { <option [value]="i.value">{{ i.label }}</option> }</select><small class="err">{{ err('insurerId') }}</small></label>
        <label>Número de póliza<input formControlName="policyNumber" /><small class="err">{{ err('policyNumber') }}</small></label>
      }
      <label>Número de acta de defunción<input formControlName="certNumber" /></label>
      <label>Archivo del acta (PDF, JPG o PNG, máx. 5 MB)<input type="file" accept=".pdf,.jpg,.jpeg,.png" (change)="file.emit($any($event.target).files?.[0] ?? null)" /></label>
      <p class="full muted">El acta es necesaria para facturar un servicio inmediato; puede cargarla después desde el detalle.</p>
    </div></div>
    <div class="card sum"><h2>Resumen</h2>
      <dl><dt>Subtotal</dt><dd>{{ totals.subtotal | colones }}</dd><dt>Exonerado</dt><dd>{{ totals.exempt | colones }}</dd><dt>IVA 13 %</dt><dd>{{ totals.tax | colones }}</dd><dt class="big">Total</dt><dd class="big">{{ totals.total | colones }}</dd></dl></div>`,
  styles: [`.grid { display: grid; grid-template-columns: 1fr 1fr; gap: 12px 16px; } .full { grid-column: 1 / -1; } label { display: flex; flex-direction: column; gap: 4px; font-size: 12px; color: var(--text-muted); }
    .sum { margin-top: 16px; } dl { display: grid; grid-template-columns: 1fr auto; gap: 4px 12px; margin: 8px 0 0; } dd { margin: 0; text-align: right; } .big { font-size: 16px; font-weight: 600; } .err { color: #e0a0a0; }
    @media (max-width: 768px) { .grid { grid-template-columns: 1fr; } }`],
})
export class StepPaymentDocsComponent {
  @Input({ required: true }) form!: FormGroup;
  @Input() insurers: Option[] = [];
  @Input() totals: Totals = { subtotal: 0, exempt: 0, tax: 0, total: 0 };
  @Output() file = new EventEmitter<File | null>();
  err(c: string): string { const k = this.form.get(['payment', c]); return k && (k.touched || k.dirty) && k.errors ? (k.errors['server'] ?? 'Campo obligatorio.') : ''; }
}
