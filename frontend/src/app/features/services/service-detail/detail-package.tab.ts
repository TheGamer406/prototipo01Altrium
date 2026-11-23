/**
 * Archivo:    frontend/src/app/features/services/service-detail/detail-package.tab.ts
 * Qué hace:   Paquete y líneas (editable antes de facturar).
 * Recibe:     Detalle.
 * Procesa:    Edita líneas con PUT /package; marca personalizado.
 * Devuelve:   Vista de la pestaña; acciones vía el Api correspondiente.
 * Se conecta: ← service-detail.page
 * Reglas:     RF-04 · RF-05
 * Estado:     Completo
 */

import { Component, EventEmitter, Input, Output, inject } from '@angular/core';
import { MatSnackBar } from '@angular/material/snack-bar';
import { ServicesApi } from '../../../core/api/services.api';
import { ServiceDetail } from '../../../core/models/services.models';
import { StatusChipComponent } from '../../../shared/components/status-chip.component';
import { ColonesPipe } from '../../../shared/pipes/colones.pipe';

@Component({
  selector: 'app-detail-package-tab',
  standalone: true,
  imports: [ColonesPipe, StatusChipComponent],
  template: `
    <div class="card">
      <p>Paquete base: <strong>{{ s.basePackage ?? '—' }}</strong> @if (s.isCustomPackage) { <span class="tag">Paquete personalizado</span> }</p>
      <table class="dense"><thead><tr><th>Componente</th><th>Cant.</th><th class="num">Precio</th><th>Exoneración</th><th class="num">Total</th><th>Estado</th><th></th></tr></thead>
        <tbody>@for (l of s.lines; track l.id) { <tr><td>{{ l.componentName }}{{ l.casketModelName ? ' · ' + l.casketModelName : '' }}</td><td>{{ l.qty }}</td><td class="num">{{ l.unitPriceCrc | colones }}</td>
          <td>{{ l.taxExemptPct }} %</td><td class="num">{{ l.lineTotalCrc | colones }}</td><td><app-status-chip [status]="l.status" kind="line" /></td>
          <td>@if (editable && l.category !== 'Casket') { <button class="btn ghost" type="button" (click)="remove(l.componentId)" [attr.aria-label]="'Quitar ' + l.componentName"><span class="ms">delete</span></button> }</td></tr> }</tbody></table>
      <p class="num total">Total {{ s.totalCrc | colones }}</p>
      @if (!editable) { <p class="muted">El paquete ya no se puede cambiar: el servicio está facturado.</p> }
    </div>`,
  styles: [`.tag { border: 1px solid var(--warning); color: var(--warning); border-radius: 999px; padding: 2px 10px; font-size: 12px; margin-left: 8px; } .total { font-size: 16px; font-weight: 600; }`],
})
export class DetailPackageTab {
  private api = inject(ServicesApi);
  private snack = inject(MatSnackBar);
  @Input({ required: true }) s!: ServiceDetail;
  @Output() changed = new EventEmitter<void>();
  get editable(): boolean { return this.s.status === 'Advance' || this.s.status === 'Registered'; }
  remove(componentId: number): void {
    this.api.setPackage(this.s.id, { basePackageId: this.s.basePackageId!, lines: this.s.lines.filter(l => l.componentId !== componentId).map(l => ({ componentId: l.componentId, casketModelId: l.casketModelId, qty: l.qty })) })
      .subscribe(() => { this.snack.open('Componente quitado. Queda registrado en la bitácora.', 'Cerrar', { duration: 4000, panelClass: 'success' }); this.changed.emit(); });
  }
}
