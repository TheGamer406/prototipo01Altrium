/**
 * Archivo:    frontend/src/app/core/api/services.api.ts
 * Qué hace:   ServicesApi: cliente HTTP tipado.
 * Recibe:     Parámetros tipados con core/models.
 * Procesa:    HttpClient contra GET/POST/PUT /api/services*, /api/prospects/export; sin lógica de
 *               negocio.
 * Devuelve:   Observables tipados: list(filters), get(id), create(req), update(id, req), activate(id,
 *               req), setPackage(id, req), uploadCertificate(id, number, file), board(id), timeline(id).
 * Se conecta: ← features · → backend · ↔ core/models/services.models.ts
 * Reglas:     RF-01 a RF-07 · RF-19
 * Estado:     Completo
 */

import { HttpClient } from '@angular/common/http';
import { Injectable, inject } from '@angular/core';
import { Observable } from 'rxjs';
import { PagedResult } from '../models/meta.models';
import { CreateServiceRequest, DeathCertificate, LineInput, ServiceBoard, ServiceDetail, ServiceFilters, ServiceListItem, TimelineEntry } from '../models/services.models';
import { toParams } from '../http/params';

@Injectable({ providedIn: 'root' })
export class ServicesApi {
  private http = inject(HttpClient);
  list(f: ServiceFilters = {}): Observable<PagedResult<ServiceListItem>> { return this.http.get<PagedResult<ServiceListItem>>('/api/services', { params: toParams({ ...f }) }); }
  get(id: string): Observable<ServiceDetail> { return this.http.get<ServiceDetail>(`/api/services/${id}`); }
  create(req: CreateServiceRequest): Observable<ServiceDetail> { return this.http.post<ServiceDetail>('/api/services', req); }
  update(id: string, req: unknown): Observable<ServiceDetail> { return this.http.put<ServiceDetail>(`/api/services/${id}`, req); }
  activate(id: string, req: { deathDate: string; scheduledAt: string }): Observable<ServiceDetail> { return this.http.post<ServiceDetail>(`/api/services/${id}/activate`, req); }
  setPackage(id: string, req: { basePackageId: number; lines: LineInput[] }): Observable<ServiceDetail> { return this.http.put<ServiceDetail>(`/api/services/${id}/package`, req); }
  uploadCertificate(id: string, number: string, file: File): Observable<DeathCertificate> {
    const body = new FormData(); body.append('number', number); body.append('file', file);
    return this.http.post<DeathCertificate>(`/api/services/${id}/death-certificate`, body);
  }
  board(id: string): Observable<ServiceBoard> { return this.http.get<ServiceBoard>(`/api/services/${id}/board`); }
  timeline(id: string): Observable<TimelineEntry[]> { return this.http.get<TimelineEntry[]>(`/api/services/${id}/timeline`); }
}
