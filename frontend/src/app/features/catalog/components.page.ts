/**
 * Archivo:    frontend/src/app/features/catalog/components.page.ts
 * Qué hace:   Componentes.
 * Recibe:     Categoría.
 * Procesa:    Crear/editar (precio, % exento, proveedor, imagen).
 * Devuelve:   Tabla + diálogo.
 * Se conecta: → CatalogApi
 * Reglas:     RF-06 · RF-10
 * Estado:     Completo
 */

import { Component, inject, signal } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { MatSnackBar } from '@angular/material/snack-bar';
import { CatalogApi } from '../../core/api/catalog.api';
import { Component as Comp } from '../../core/models/catalog.models';
import { ColonesPipe } from '../../shared/pipes/colones.pipe';

const CATS: [string, string][] = [['Casket', 'Cofre'], ['Flowers', 'Flores'], ['Hearse', 'Carroza'], ['Chapel', 'Capilla'], ['Church', 'Iglesia'], ['BodyPreparation', 'Preparación del cuerpo'], ['Cremation', 'Cremación'], ['Catering', 'Catering'], ['Music', 'Música'], ['Extra', 'Adicional']];
const blank = (): Comp => ({ id: 0, name: '', category: 'Extra', priceCrc: 0, taxExemptPct: 0, requiresProvider: false, providerType: null, imagePath: null, active: true });

@Component({
  selector: 'app-components-page',
  standalone: true,
  imports: [FormsModule, ColonesPipe],
  template: `
    <div class="bar"><button class="btn primary" type="button" (click)="edit.set(blank())"><span class="ms">add</span>Nuevo componente</button></div>
    @if (edit(); as e) {
      <form class="card form" (ngSubmit)="save()">
        <label>Nombre<input name="n" [(ngModel)]="e.name" required /></label>
        <label>Categoría<select name="c" [(ngModel)]="e.category">@for (c of cats; track c[0]) { <option [value]="c[0]">{{ c[1] }}</option> }</select></label>
        <label>Precio (₡)<input name="p" type="number" min="0" [(ngModel)]="e.priceCrc" /></label>
        <label>Exoneración de IVA<select name="x" [(ngModel)]="e.taxExemptPct"><option [ngValue]="0">0 %</option><option [ngValue]="50">50 %</option><option [ngValue]="100">100 %</option></select></label>
        <label class="chk"><input name="r" type="checkbox" [(ngModel)]="e.requiresProvider" /> Requiere proveedor externo</label>
        @if (e.requiresProvider) { <label>Tipo de proveedor<select name="t" [(ngModel)]="e.providerType"><option value="Flowers">Floristería</option><option value="Catering">Catering</option><option value="Music">Música</option></select></label> }
        <label class="chk"><input name="a" type="checkbox" [(ngModel)]="e.active" /> Activo</label>
        <div class="acts"><button class="btn primary" type="submit">Guardar</button><button class="btn ghost" type="button" (click)="edit.set(null)">Cancelar</button></div>
      </form> }
    <table class="dense"><thead><tr><th>Componente</th><th>Categoría</th><th class="num">Precio</th><th>Exoneración</th><th>Proveedor</th><th>Estado</th><th></th></tr></thead>
      <tbody>@for (c of items(); track c.id) { <tr><td>{{ c.name }}</td><td>{{ catName(c.category) }}</td><td class="num">{{ c.priceCrc | colones }}</td><td>{{ c.taxExemptPct }} %</td><td>{{ c.requiresProvider ? c.providerType : '—' }}</td><td>{{ c.active ? 'Activo' : 'Inactivo' }}</td>
        <td><button class="btn ghost" type="button" (click)="edit.set({ ...c })" [attr.aria-label]="'Editar ' + c.name"><span class="ms">edit</span></button></td></tr> }</tbody></table>`,
  styles: [`.bar { margin-bottom: 12px; } .form { display: grid; grid-template-columns: repeat(3, 1fr); gap: 12px; margin-bottom: 16px; } label { display: flex; flex-direction: column; gap: 4px; font-size: 12px; color: var(--text-muted); } .chk { flex-direction: row; align-items: center; color: var(--text); } .chk input { height: auto; } .acts { display: flex; gap: 8px; align-items: end; }`],
})
export class ComponentsPage {
  private api = inject(CatalogApi);
  private snack = inject(MatSnackBar);
  readonly cats = CATS; readonly blank = blank;
  readonly items = signal<Comp[]>([]);
  readonly edit = signal<Comp | null>(null);
  constructor() { this.load(); }
  catName(c: string): string { return CATS.find(x => x[0] === c)?.[1] ?? c; }
  private load(): void { this.api.components().subscribe(r => this.items.set(r)); }
  save(): void { this.api.saveComponent(this.edit()!).subscribe(() => { this.snack.open('Componente guardado.', 'Cerrar', { duration: 3000, panelClass: 'success' }); this.edit.set(null); this.load(); }); }
}
