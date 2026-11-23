/**
 * Archivo:    frontend/src/app/core/api/analytics.api.ts
 * Qué hace:   AnalyticsApi: cliente HTTP tipado.
 * Recibe:     Parámetros tipados con core/models.
 * Procesa:    HttpClient contra /api/analytics/*; sin lógica de negocio.
 * Devuelve:   Observables tipados: sales(from, to, b), stageTimes(from, to), zones(year).
 * Se conecta: ← features · → backend · ↔ core/models/analytics.models.ts
 * Reglas:     RF-32 a RF-34
 * Estado:     Completo
 */

import { HttpClient } from '@angular/common/http';
import { Injectable, inject } from '@angular/core';
import { Observable } from 'rxjs';
import { SalesResponse, StageTimes, Zones } from '../models/analytics.models';
import { toParams } from '../http/params';

@Injectable({ providedIn: 'root' })
export class AnalyticsApi {
  private http = inject(HttpClient);
  sales(from?: string, to?: string, branch?: number | null): Observable<SalesResponse> { return this.http.get<SalesResponse>('/api/analytics/sales', { params: toParams({ from, to, branch }) }); }
  stageTimes(from?: string, to?: string): Observable<StageTimes> { return this.http.get<StageTimes>('/api/analytics/stage-times', { params: toParams({ from, to }) }); }
  zones(year?: number): Observable<Zones> { return this.http.get<Zones>('/api/analytics/zones', { params: toParams({ year }) }); }
}
