/**
 * Archivo:    frontend/src/app/core/api/meta.api.ts
 * Qué hace:   MetaApi: cliente HTTP tipado.
 * Recibe:     Parámetros tipados con core/models.
 * Procesa:    HttpClient contra /api/meta/*; sin lógica de negocio.
 * Devuelve:   Observables tipados: branches(), roles(), enums().
 * Se conecta: ← features · → backend · ↔ core/models/meta.models.ts
 * Reglas:     RF-30
 * Estado:     Completo
 */

import { HttpClient } from '@angular/common/http';
import { Injectable, inject } from '@angular/core';
import { Observable } from 'rxjs';
import { Branch, MetaEnums, Option, ProviderMeta, RoleOption } from '../models/meta.models';

@Injectable({ providedIn: 'root' })
export class MetaApi {
  private http = inject(HttpClient);
  branches(): Observable<Branch[]> { return this.http.get<Branch[]>('/api/meta/branches'); }
  roles(): Observable<RoleOption[]> { return this.http.get<RoleOption[]>('/api/meta/roles'); }
  enums(): Observable<MetaEnums> { return this.http.get<MetaEnums>('/api/meta/enums'); }
  providers(): Observable<ProviderMeta[]> { return this.http.get<ProviderMeta[]>('/api/meta/providers'); }
  insurers(): Observable<Option[]> { return this.http.get<Option[]>('/api/meta/insurers'); }
}
