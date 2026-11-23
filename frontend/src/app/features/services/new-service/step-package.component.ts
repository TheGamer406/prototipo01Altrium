/**
 * Archivo:    frontend/src/app/features/services/new-service/step-package.component.ts
 * Qué hace:   Paso 3: elegir paquete base.
 * Recibe:     PackageDto[] (4 tarjetas con imagen velorioMuestraN).
 * Procesa:    Al elegir, carga las líneas del paquete en el formulario.
 * Devuelve:   Cambios en el FormGroup del asistente (no llama al API por su cuenta).
 * Se conecta: ← new-service-wizard · → CatalogApi / LogisticsApi (solo lectura)
 * Reglas:     RF-04
 * Estado:     Completo
 */

import { Component, EventEmitter, Input, Output } from '@angular/core';
import { Package } from '../../../core/models/catalog.models';
import { ColonesPipe } from '../../../shared/pipes/colones.pipe';

@Component({
  selector: 'app-step-package',
  standalone: true,
  imports: [ColonesPipe],
  template: `
    <div class="cards">
      @for (p of packages; track p.id) {
        <button type="button" class="card pk" [class.sel]="p.id === selectedId" (click)="pick.emit(p)" [attr.aria-pressed]="p.id === selectedId">
          @if (p.imagePath) { <img [src]="'/' + p.imagePath" alt="" /> }
          <strong>{{ p.name }}</strong><span class="price">{{ p.priceCrc | colones }}</span>
          <span class="muted">{{ p.description }}</span><span class="muted">{{ p.items.length }} componentes</span>
        </button>
      }
    </div>`,
  styles: [`.cards { display: grid; grid-template-columns: repeat(2, 1fr); gap: 12px; } .pk { text-align: left; display: flex; flex-direction: column; gap: 4px; cursor: pointer; color: inherit; font: inherit; transition: border-color var(--transition); }
    .pk.sel { border-color: var(--accent); background: var(--accent-soft); } .pk img { width: 100%; height: 120px; object-fit: cover; border-radius: var(--radius); } .price { font-size: 18px; font-weight: 600; }`],
})
export class StepPackageComponent {
  @Input() packages: Package[] = [];
  @Input() selectedId: number | null = null;
  @Output() pick = new EventEmitter<Package>();
}
