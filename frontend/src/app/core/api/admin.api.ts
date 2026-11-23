/**
 * Archivo:    frontend/src/app/core/api/admin.api.ts
 * Qué hace:   AdminApi: cliente HTTP tipado.
 * Recibe:     Parámetros tipados con core/models.
 * Procesa:    HttpClient contra /api/admin/*; sin lógica de negocio.
 * Devuelve:   Observables tipados: reset(), refreshAnalytics(), simulateConcurrentAccept(),
 *               rejectNextInvoice(), coreDown(minutes).
 * Se conecta: ← features · → backend · ↔ core/models/admin.models.ts
 * Reglas:     —
 * Estado:     Completo
 */

import { HttpClient } from '@angular/common/http';
import { Injectable, inject } from '@angular/core';
import { Observable } from 'rxjs';
import { SeedSummary, SimulationResult } from '../models/admin.models';

@Injectable({ providedIn: 'root' })
export class AdminApi {
  private http = inject(HttpClient);
  reset(): Observable<SeedSummary> { return this.http.post<SeedSummary>('/api/admin/reset', {}); }
  refreshAnalytics(): Observable<{ refreshedAt: string }> { return this.http.post<{ refreshedAt: string }>('/api/admin/refresh-analytics', {}); }
  simulateConcurrentAccept(): Observable<SimulationResult> { return this.http.post<SimulationResult>('/api/admin/simulate/concurrent-accept', {}); }
  rejectNextInvoice(): Observable<SimulationResult> { return this.http.post<SimulationResult>('/api/admin/simulate/next-invoice-reject', {}); }
  coreDown(minutes = 5): Observable<SimulationResult> { return this.http.post<SimulationResult>(`/api/admin/simulate/core-down?minutes=${minutes}`, {}); }
}
