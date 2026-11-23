/**
 * Archivo:    frontend/src/app/features/catalog/casket-models.page.ts
 * Qué hace:   Modelos de cofre.
 * Recibe:     —
 * Procesa:    Crear/editar.
 * Devuelve:   Cuadrícula con imagen.
 * Se conecta: → CatalogApi
 * Reglas:     RF-06
 * Estado:     Completo
 */

import { Component, inject, signal } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { MatSnackBar } from '@angular/material/snack-bar';
import { CatalogApi } from '../../core/api/catalog.api';
import { CasketModel } from '../../core/models/catalog.models';
import { ColonesPipe } from '../../shared/pipes/colones.pipe';

@Component({
  selector: 'app-casket-models-page',
  standalone: true,
  imports: [FormsModule, ColonesPipe],
  template: `
    <div class="bar"><button class="btn primary" type="button" (click)="newModel()"><span class="ms">add</span>Nuevo modelo</button></div>
    @if (edit(); as e) {
      <form class="card form" (ngSubmit)="save()">
        <label>Código<input name="s" [(ngModel)]="e.sku" required /></label><label>Nombre<input name="n" [(ngModel)]="e.name" required /></label><label>Material<input name="m" [(ngModel)]="e.material" /></label>
        <label>Origen<select name="o" [(ngModel)]="e.origin"><option value="Nacional">Nacional</option><option value="Importado">Importado</option></select></label>
        <label>Precio (₡)<input name="p" type="number" min="0" [(ngModel)]="e.priceCrc" /></label>
        <label>Imagen<select name="i" [(ngModel)]="e.imagePath">@for (n of imgs; track n) { <option [value]="'img/cofres/cofre' + n + '.webp'">Cofre {{ n }}</option> }</select></label>
        <div class="acts"><button class="btn primary" type="submit">Guardar</button><button class="btn ghost" type="button" (click)="edit.set(null)">Cancelar</button></div></form> }
    <div class="grid">@for (m of items(); track m.id) { <article class="card"><img [src]="'/' + m.imagePath" alt="" /><strong>{{ m.name }}</strong><span class="muted">{{ m.sku }} · {{ m.material }} · {{ m.origin }}</span><span>{{ m.priceCrc | colones }}</span>
      <button class="btn ghost" type="button" (click)="editCopy(m)" [attr.aria-label]="'Editar ' + m.name"><span class="ms">edit</span></button></article> }</div>`,
  styles: [`.bar { margin-bottom: 12px; } .form { display: grid; grid-template-columns: repeat(3, 1fr); gap: 12px; margin-bottom: 16px; } label { display: flex; flex-direction: column; gap: 4px; font-size: 12px; color: var(--text-muted); } .acts { display: flex; gap: 8px; align-items: end; }
    .grid { display: grid; grid-template-columns: repeat(auto-fill, minmax(190px, 1fr)); gap: 12px; } article { display: flex; flex-direction: column; gap: 4px; padding: 10px; } img { height: 90px; object-fit: contain; background: var(--bg); border-radius: var(--radius); }`],
})
export class CasketModelsPage {
  private api = inject(CatalogApi);
  private snack = inject(MatSnackBar);
  readonly imgs = [1, 2, 3, 4, 5, 6, 7, 8, 9, 10];
  readonly items = signal<CasketModel[]>([]);
  readonly edit = signal<CasketModel | null>(null);
  constructor() { this.load(); }
  editCopy(m: CasketModel): void { this.edit.set({ ...m }); }
  newModel(): void { this.edit.set({ id: 0, sku: '', name: '', material: '', origin: 'Nacional', priceCrc: 0, imagePath: 'img/cofres/cofre1.webp', stockInBranch: null }); }
  private load(): void { this.api.casketModels(null).subscribe(r => this.items.set(r)); }
  save(): void { this.api.saveCasketModel(this.edit()!).subscribe(() => { this.snack.open('Modelo guardado.', 'Cerrar', { duration: 3000, panelClass: 'success' }); this.edit.set(null); this.load(); }); }
}
