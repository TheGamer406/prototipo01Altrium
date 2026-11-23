/**
 * Archivo:    frontend/src/app/features/services/new-service/step-casket.component.ts
 * Qué hace:   Paso 4: personalizar paquete y elegir cofre con imagen y stock de la sede.
 * Recibe:     Líneas actuales + catálogo filtrable (florales, catering y música con imagen) +
 *               CasketModelDto[] con stock.
 * Procesa:    Quitar/agregar componentes; cuadrícula de cofres (sin stock = deshabilitado con
 *               etiqueta); etiqueta 'Paquete personalizado' si difiere del base.
 * Devuelve:   Cambios en el FormGroup del asistente (no llama al API por su cuenta).
 * Se conecta: ← new-service-wizard · → CatalogApi / LogisticsApi (solo lectura)
 * Reglas:     RF-04 · RF-05 · RF-26
 * Estado:     Completo
 */

import { Component, EventEmitter, Input, Output } from '@angular/core';
import { CasketModel, Component as Comp } from '../../../core/models/catalog.models';
import { LineInput } from '../../../core/models/services.models';
import { ColonesPipe } from '../../../shared/pipes/colones.pipe';

@Component({
  selector: 'app-step-casket',
  standalone: true,
  imports: [ColonesPipe],
  template: `
    @if (custom) { <p class="tag">Paquete personalizado</p> }
    <h2>Componentes del servicio</h2>
    <ul class="lines">
      @for (l of lines; track l.componentId) {
        <li><span>{{ name(l.componentId) }}</span>
          <span class="muted">{{ l.qty }} ×</span>
          @if (!isCasket(l.componentId)) { <button class="btn ghost" type="button" (click)="remove.emit(l.componentId)" [attr.aria-label]="'Quitar ' + name(l.componentId)"><span class="ms">delete</span></button> }
        </li>
      }
    </ul>
    <div class="add"><select #sel aria-label="Agregar componente"><option value="">Agregar componente…</option>
      @for (c of available; track c.id) { <option [value]="c.id">{{ c.name }} · {{ c.priceCrc | colones }}</option> }</select>
      <button class="btn" type="button" (click)="sel.value && add.emit(+sel.value); sel.value=''">Agregar</button></div>
    <div class="thumbs">@for (c of available; track c.id) { @if (c.imagePath) { <figure (click)="add.emit(c.id)"><img [src]="'/' + c.imagePath" [alt]="c.name" /><figcaption>{{ c.name }}</figcaption></figure> } }</div>

    @if (hasCasket) {
      <h2>Cofre</h2>
      <div class="caskets">
        @for (m of caskets; track m.id) {
          <button type="button" class="card ck" [disabled]="(m.stockInBranch ?? 0) <= 0" [class.sel]="m.id === casketId" (click)="chooseCasket.emit(m.id)">
            <img [src]="'/' + m.imagePath" [alt]="m.name" /><strong>{{ m.name }}</strong>
            <span class="muted">{{ m.material }} · {{ m.origin }}</span><span>{{ m.priceCrc | colones }}</span>
            @if ((m.stockInBranch ?? 0) <= 0) { <span class="out">Sin existencias</span> } @else { <span class="muted">{{ m.stockInBranch }} en la sede</span> }
          </button>
        }
      </div>
    }`,
  styles: [`.tag { display: inline-block; border: 1px solid var(--warning); color: var(--warning); border-radius: 999px; padding: 2px 10px; font-size: 12px; margin: 0 0 8px; }
    .lines { list-style: none; padding: 0; margin: 8px 0; } .lines li { display: flex; align-items: center; gap: 8px; height: var(--row-h); border-bottom: 1px solid var(--border); } .lines li span:first-child { flex: 1; }
    .add { display: flex; gap: 8px; margin: 8px 0; } .add select { flex: 1; } h2 { margin: 16px 0 4px; }
    .thumbs { display: flex; gap: 8px; flex-wrap: wrap; } figure { margin: 0; width: 84px; font-size: 11px; text-align: center; cursor: pointer; color: var(--text-muted); } figure img { width: 84px; height: 64px; object-fit: cover; border-radius: var(--radius); }
    .caskets { display: grid; grid-template-columns: repeat(3, 1fr); gap: 10px; } .ck { display: flex; flex-direction: column; gap: 2px; text-align: left; cursor: pointer; color: inherit; font: inherit; padding: 8px; }
    .ck img { width: 100%; height: 110px; object-fit: contain; background: var(--bg); border-radius: var(--radius); } .ck.sel { border-color: var(--accent); background: var(--accent-soft); }
    .ck:disabled { opacity: .45; cursor: not-allowed; } .out { color: #e0a0a0; font-size: 12px; }
    @media (max-width: 1024px) { .caskets { grid-template-columns: repeat(2, 1fr); } } @media (max-width: 480px) { .caskets { grid-template-columns: 1fr; } }`],
})
export class StepCasketComponent {
  @Input() lines: LineInput[] = [];
  @Input() components: Comp[] = [];
  @Input() caskets: CasketModel[] = [];
  @Input() custom = false;
  @Output() add = new EventEmitter<number>();
  @Output() remove = new EventEmitter<number>();
  @Output() chooseCasket = new EventEmitter<number>();

  name(id: number): string { return this.components.find(c => c.id === id)?.name ?? ''; }
  isCasket(id: number): boolean { return this.components.find(c => c.id === id)?.category === 'Casket'; }
  get hasCasket(): boolean { return this.lines.some(l => this.isCasket(l.componentId)); }
  get casketId(): number | null { return this.lines.find(l => this.isCasket(l.componentId))?.casketModelId ?? null; }
  get available(): Comp[] { return this.components.filter(c => c.category !== 'Casket' && !this.lines.some(l => l.componentId === c.id)); }
}
