/**
 * Archivo:    frontend/src/app/core/api/catalog.api.ts
 * Qué hace:   CatalogApi: cliente HTTP tipado.
 * Recibe:     Parámetros tipados con core/models.
 * Procesa:    HttpClient contra /api/components, /api/packages, /api/casket-models; sin lógica de
 *               negocio.
 * Devuelve:   Observables tipados: components(f), packages(), casketModels(branch), save*(dto).
 * Se conecta: ← features · → backend · ↔ core/models/catalog.models.ts
 * Reglas:     RF-04 · RF-06
 * Estado:     Parcial: sin save* (fase 5)
 */

import { HttpClient } from '@angular/common/http';
import { Injectable, inject } from '@angular/core';
import { Observable } from 'rxjs';
import { CasketModel, Component, Package } from '../models/catalog.models';
import { toParams } from '../http/params';

@Injectable({ providedIn: 'root' })
export class CatalogApi {
  private http = inject(HttpClient);
  components(f: { category?: string; active?: boolean } = {}): Observable<Component[]> { return this.http.get<Component[]>('/api/components', { params: toParams(f) }); }
  packages(): Observable<Package[]> { return this.http.get<Package[]>('/api/packages'); }
  casketModels(branch?: number | null): Observable<CasketModel[]> { return this.http.get<CasketModel[]>('/api/casket-models', { params: toParams({ branch }) }); }
}
