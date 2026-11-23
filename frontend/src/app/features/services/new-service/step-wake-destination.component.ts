/**
 * Archivo:    frontend/src/app/features/services/new-service/step-wake-destination.component.ts
 * Qué hace:   Paso 5: velación y destino final.
 * Recibe:     FormGroup 'wake' + calendario de capillas.
 * Procesa:    Capilla (muestra disponibilidad) | iglesia | casa (pide dirección); cementerio (nombre)
 *               | cremación; fecha y hora.
 * Devuelve:   Cambios en el FormGroup del asistente (no llama al API por su cuenta).
 * Se conecta: ← new-service-wizard · → CatalogApi / LogisticsApi (solo lectura)
 * Reglas:     RF-07 · RF-20
 * Estado:     Completo
 */

import { Component, EventEmitter, Input, Output } from '@angular/core';
import { FormGroup, ReactiveFormsModule } from '@angular/forms';
import { ChapelCalendar } from '../../../core/models/logistics.models';
import { CrDatePipe } from '../../../shared/pipes/cr-date.pipe';

@Component({
  selector: 'app-step-wake-destination',
  standalone: true,
  imports: [ReactiveFormsModule, CrDatePipe],
  template: `
    <div [formGroup]="form" class="grid"><div formGroupName="wake" class="grid full">
      <label>Fecha de la velación<input type="date" formControlName="date" (change)="changed.emit()" /><small class="err">{{ err('date') }}</small></label>
      <label>Hora<input type="time" formControlName="time" (change)="changed.emit()" /></label>
      <label class="full">Lugar de la velación<select formControlName="place" (change)="changed.emit()">
        <option value="Chapel">Capilla de la sede</option><option value="Church">Iglesia</option><option value="Home">Casa de habitación</option></select></label>
      @if (form.get('wake.place')?.value === 'Home') { <label class="full">Dirección<input formControlName="address" /><small class="err">{{ err('address') }}</small></label> }
      @if (form.get('wake.place')?.value === 'Chapel') {
        <div class="full"><span class="muted">Capillas (24 h desde la hora indicada)</span>
          @if (!calendar) { <p class="muted">Indique fecha y hora para ver la disponibilidad.</p> }
          @for (c of calendar?.chapels ?? []; track c.id) {
            <label class="chapel" [class.busy]="c.bookings.length > 0">
              <input type="radio" formControlName="chapelId" [value]="c.id" [attr.disabled]="c.bookings.length ? true : null" />
              <strong>{{ c.name }}</strong><span class="muted">aforo {{ c.capacity }}</span>
              @if (c.bookings.length) { <span class="out">Ocupada: {{ c.bookings[0].serviceCode }} {{ c.bookings[0].from | crDate:'datetime' }}</span> } @else { <span class="ok">Disponible</span> }
            </label>
          }
          <small class="err">{{ err('chapelId') }}</small></div>
      }
      <label class="full">Destino final<select formControlName="destination"><option value="Cemetery">Cementerio</option><option value="Cremation">Cremación</option></select></label>
      @if (form.get('wake.destination')?.value === 'Cemetery') { <label class="full">Cementerio<input formControlName="cemeteryName" /><small class="err">{{ err('cemeteryName') }}</small></label> }
    </div></div>`,
  styles: [`.grid { display: grid; grid-template-columns: 1fr 1fr; gap: 12px 16px; } .full { grid-column: 1 / -1; } label { display: flex; flex-direction: column; gap: 4px; font-size: 12px; color: var(--text-muted); }
    .chapel { flex-direction: row; align-items: center; gap: 10px; color: var(--text); font-size: 13px; height: var(--row-h); border-bottom: 1px solid var(--border); } .chapel input { height: auto; } .chapel .ok { color: var(--success); margin-left: auto; } .chapel .out { color: #e0a0a0; margin-left: auto; }
    .busy { opacity: .7; } .err { color: #e0a0a0; min-height: 14px; }`],
})
export class StepWakeDestinationComponent {
  @Input({ required: true }) form!: FormGroup;
  @Input() calendar: ChapelCalendar | null = null;
  @Output() changed = new EventEmitter<void>();
  err(c: string): string {
    const k = this.form.get(['wake', c]);
    if (!k || !(k.touched || k.dirty) || !k.errors) return '';
    return k.errors['server'] ?? 'Campo obligatorio.';
  }
}
