/**
 * Archivo:    frontend/src/app/core/api/oversight.api.ts
 * Qué hace:   OversightApi: cliente HTTP tipado.
 * Recibe:     Parámetros tipados con core/models.
 * Procesa:    HttpClient contra /api/overview, /api/audit, /api/alerts, /api/messages, /api/settings;
 *               sin lógica de negocio.
 * Devuelve:   Observables tipados: overview(), audit(f), alerts(b), ack(id), messages(f), settings(),
 *               saveSettings(map).
 * Se conecta: ← features · → backend · ↔ core/models/oversight.models.ts
 * Reglas:     RF-29 · RF-31
 * Estado:     Completo
 */

import { HttpClient } from '@angular/common/http';
import { Injectable, inject } from '@angular/core';
import { Observable } from 'rxjs';
import { Alert, AuditEntry, Message, Overview } from '../models/oversight.models';
import { PagedResult } from '../models/meta.models';
import { toParams } from '../http/params';

@Injectable({ providedIn: 'root' })
export class OversightApi {
  private http = inject(HttpClient);
  overview(): Observable<Overview> { return this.http.get<Overview>('/api/overview'); }
  audit(f: { entity?: string; serviceId?: string; from?: string; to?: string; actor?: string; page?: number } = {}): Observable<PagedResult<AuditEntry>> { return this.http.get<PagedResult<AuditEntry>>('/api/audit', { params: toParams(f) }); }
  alerts(branch?: number | null, all = false): Observable<Alert[]> { return this.http.get<Alert[]>('/api/alerts', { params: toParams({ branch, all }) }); }
  ack(id: number): Observable<void> { return this.http.post<void>(`/api/alerts/${id}/ack`, {}); }
  messages(f: { serviceId?: string; kind?: string; page?: number } = {}): Observable<PagedResult<Message>> { return this.http.get<PagedResult<Message>>('/api/messages', { params: toParams(f) }); }
  settings(): Observable<Record<string, string>> { return this.http.get<Record<string, string>>('/api/settings'); }
  saveSettings(map: Record<string, string>): Observable<Record<string, string>> { return this.http.put<Record<string, string>>('/api/settings', map); }
}
