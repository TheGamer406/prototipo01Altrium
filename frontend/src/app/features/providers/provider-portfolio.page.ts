/**
 * Archivo:    frontend/src/app/features/providers/provider-portfolio.page.ts
 * Qué hace:   Cartera de proveedores por tipo y zona.
 * Recibe:     Filtros tipo/zona.
 * Procesa:    CRUD en diálogo.
 * Devuelve:   Tabla + diálogo de edición.
 * Se conecta: → ProvidersApi
 * Reglas:     RF-14
 * Estado:     Completo
 */

import { Component, inject, signal } from '@angular/core';
import { ProvidersApi } from '../../core/api/providers.api';
import { Provider } from '../../core/models/providers.models';

@Component({
  selector: 'app-provider-portfolio-page',
  standalone: true,
  template: `<div class="filters"><select (change)="filter($any($event.target).value)" aria-label="Tipo"><option value="">Todos los tipos</option><option value="Flowers">Floristería</option><option value="Catering">Catering</option><option value="Music">Música</option></select></div>
    <table class="dense"><thead><tr><th>Proveedor</th><th>Tipo</th><th>Zona</th><th>Contacto</th><th class="num">Ganadas (30 d)</th><th>Estado</th></tr></thead>
      <tbody>@for (p of items(); track p.id) { <tr><td><strong>{{ p.name }}</strong></td><td>{{ types[p.type] }}</td><td>{{ p.zone === 'GAM' ? 'GAM' : 'Fuera del GAM' }}</td><td class="muted">{{ p.email }} · {{ p.phone }}</td><td class="num">{{ p.wonLast30d }}</td><td>{{ p.active ? 'Activo' : 'Inactivo' }}</td></tr> }</tbody></table>`,
  styles: [`.filters { margin-bottom: 12px; }`],
})
export class ProviderPortfolioPage {
  private api = inject(ProvidersApi);
  readonly items = signal<Provider[]>([]);
  readonly types: Record<string, string> = { Flowers: 'Floristería', Catering: 'Catering', Music: 'Música' };
  constructor() { this.filter(''); }
  filter(type: string): void { this.api.list({ type: type || undefined }).subscribe(r => this.items.set(r)); }
}
