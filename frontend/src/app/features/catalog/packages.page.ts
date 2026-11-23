/**
 * Archivo:    frontend/src/app/features/catalog/packages.page.ts
 * Qué hace:   Paquetes.
 * Recibe:     —
 * Procesa:    Crear/editar paquete y sus componentes; precio calculado.
 * Devuelve:   Tarjetas + diálogo.
 * Se conecta: → CatalogApi
 * Reglas:     RF-04 · RF-06
 * Estado:     Completo
 */

import { Component, inject, signal } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { MatSnackBar } from '@angular/material/snack-bar';
import { CatalogApi } from '../../core/api/catalog.api';
import { Component as Comp, Package } from '../../core/models/catalog.models';
import { ColonesPipe } from '../../shared/pipes/colones.pipe';

@Component({
  selector: 'app-packages-page',
  standalone: true,
  imports: [FormsModule, ColonesPipe],
  template: `
    <div class="bar"><button class="btn primary" type="button" (click)="start({ id: 0, name: '', description: '', priceCrc: 0, items: [], imagePath: 'img/otros/velorioMuestra1.webp', active: true })"><span class="ms">add</span>Nuevo paquete</button></div>
    @if (edit(); as e) {
      <form class="card form" (ngSubmit)="save()">
        <label>Nombre<input name="n" [(ngModel)]="e.name" required /></label><label>Descripción<input name="d" [(ngModel)]="e.description" /></label>
        <label class="chk"><input name="a" type="checkbox" [(ngModel)]="e.active" /> Activo</label>
        <h2>Componentes · {{ total() | colones }}</h2>
        @for (i of e.items; track $index) { <div class="line"><span>{{ i.name }}</span><input type="number" min="1" [name]="'q' + $index" [(ngModel)]="i.qty" aria-label="Cantidad" /><button class="btn ghost" type="button" (click)="remove($index)" aria-label="Quitar"><span class="ms">delete</span></button></div> }
        <div class="line"><select name="add" [(ngModel)]="toAdd"><option [ngValue]="null">Agregar componente…</option>@for (c of comps(); track c.id) { <option [ngValue]="c.id">{{ c.name }} · {{ c.priceCrc | colones }}</option> }</select><button class="btn" type="button" (click)="add()">Agregar</button></div>
        <p class="muted">Editar un paquete no cambia los servicios ya registrados. El precio es la suma de sus componentes.</p>
        <div class="acts"><button class="btn primary" type="submit">Guardar</button><button class="btn ghost" type="button" (click)="edit.set(null)">Cancelar</button></div>
      </form> }
    <div class="cards">@for (p of items(); track p.id) { <article class="card"><header><strong>{{ p.name }}</strong><button class="btn ghost" type="button" (click)="start(clone(p))" [attr.aria-label]="'Editar ' + p.name"><span class="ms">edit</span></button></header>
      <span class="price">{{ p.priceCrc | colones }}</span><span class="muted">{{ p.description }}</span><span class="muted">{{ p.items.length }} componentes · {{ p.active ? 'Activo' : 'Inactivo' }}</span></article> }</div>`,
  styles: [`.bar { margin-bottom: 12px; } .form { display: flex; flex-direction: column; gap: 10px; margin-bottom: 16px; max-width: 640px; } label { display: flex; flex-direction: column; gap: 4px; font-size: 12px; color: var(--text-muted); } .chk { flex-direction: row; align-items: center; color: var(--text); } .chk input { height: auto; }
    .line { display: flex; gap: 8px; align-items: center; } .line span, .line select { flex: 1; } .line input { width: 70px; } .acts { display: flex; gap: 8px; } .cards { display: grid; grid-template-columns: repeat(auto-fill, minmax(240px, 1fr)); gap: 12px; } article { display: flex; flex-direction: column; gap: 4px; } header { display: flex; justify-content: space-between; align-items: center; } .price { font-size: 20px; font-weight: 600; } h2 { margin: 8px 0 0; }`],
})
export class PackagesPage {
  private api = inject(CatalogApi);
  private snack = inject(MatSnackBar);
  readonly items = signal<Package[]>([]);
  readonly comps = signal<Comp[]>([]);
  readonly edit = signal<Package | null>(null);
  toAdd: number | null = null;
  constructor() { this.load(); this.api.components({ active: true }).subscribe(c => this.comps.set(c)); }
  private load(): void { this.api.packages().subscribe(p => this.items.set(p)); }
  clone(p: Package): Package { return { ...p, items: p.items.map(i => ({ ...i })) }; }
  start(p: Package): void { this.edit.set(p); }
  total(): number { return (this.edit()?.items ?? []).reduce((s, i) => s + i.qty * (this.comps().find(c => c.id === i.componentId)?.priceCrc ?? i.priceCrc), 0); }
  add(): void {
    const c = this.comps().find(x => x.id === this.toAdd); const e = this.edit();
    if (!c || !e || e.items.some(i => i.componentId === c.id)) return;
    e.items.push({ componentId: c.id, name: c.name, category: c.category, qty: 1, priceCrc: c.priceCrc, taxExemptPct: c.taxExemptPct, requiresProvider: c.requiresProvider }); this.toAdd = null;
  }
  remove(i: number): void { this.edit()!.items.splice(i, 1); }
  save(): void { this.api.savePackage(this.edit()!).subscribe(() => { this.snack.open('Paquete guardado.', 'Cerrar', { duration: 3000, panelClass: 'success' }); this.edit.set(null); this.load(); }); }
}
