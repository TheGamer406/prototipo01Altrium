/**
 * Archivo:    frontend/src/app/features/services/service-detail/detail-documents.tab.ts
 * Qué hace:   Acta de defunción.
 * Recibe:     Detalle.
 * Procesa:    Registrar o ver el acta.
 * Devuelve:   Vista de la pestaña; acciones vía el Api correspondiente.
 * Se conecta: ← service-detail.page
 * Reglas:     RF-03
 * Estado:     Completo
 */

import { Component, EventEmitter, Input, Output, inject } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { MatSnackBar } from '@angular/material/snack-bar';
import { ServicesApi } from '../../../core/api/services.api';
import { ServiceDetail } from '../../../core/models/services.models';
import { CrDatePipe } from '../../../shared/pipes/cr-date.pipe';

@Component({
  selector: 'app-detail-documents-tab',
  standalone: true,
  imports: [FormsModule, CrDatePipe],
  template: `
    <div class="card">
      <h2>Acta de defunción</h2>
      @if (s.deathCertificate; as c) {
        <p>Acta <strong>{{ c.number }}</strong> registrada por {{ c.registeredBy }} el {{ c.registeredAt | crDate }}. <a [href]="c.fileUrl" target="_blank">Ver archivo</a></p>
      } @else { <p class="muted">{{ s.kind === 'Immediate' ? 'Falta el acta: es obligatoria para facturar.' : 'Servicio anticipado: el acta se carga al activarlo.' }}</p> }
      <div class="row"><input [(ngModel)]="number" placeholder="Número de acta" aria-label="Número de acta" />
        <input type="file" accept=".pdf,.jpg,.jpeg,.png" (change)="file = $any($event.target).files?.[0] ?? null" />
        <button class="btn primary" type="button" [disabled]="!number || !file" (click)="upload()">{{ s.deathCertificate ? 'Reemplazar' : 'Cargar acta' }}</button></div>
    </div>`,
  styles: [`.row { display: flex; gap: 8px; align-items: center; margin-top: 8px; }`],
})
export class DetailDocumentsTab {
  private api = inject(ServicesApi);
  private snack = inject(MatSnackBar);
  @Input({ required: true }) s!: ServiceDetail;
  @Output() changed = new EventEmitter<void>();
  number = ''; file: File | null = null;
  upload(): void { this.api.uploadCertificate(this.s.id, this.number, this.file!).subscribe(() => { this.snack.open('Acta registrada.', 'Cerrar', { duration: 3000, panelClass: 'success' }); this.number = ''; this.changed.emit(); }); }
}
