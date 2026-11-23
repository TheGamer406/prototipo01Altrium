/**
 * Archivo:    frontend/src/app/core/api/providers.api.ts
 * Qué hace:   ProvidersApi: cliente HTTP tipado.
 * Recibe:     Parámetros tipados con core/models.
 * Procesa:    HttpClient contra /api/providers*, /api/offers*; sin lógica de negocio.
 * Devuelve:   Observables tipados: list(f), save(dto), opportunities(branch), inbox(providerId),
 *               accept(offerId).
 * Se conecta: ← features · → backend · ↔ core/models/providers.models.ts
 * Reglas:     RF-14 a RF-17
 * Estado:     Completo
 */

import { HttpClient } from '@angular/common/http';
import { Injectable, inject } from '@angular/core';
import { Observable } from 'rxjs';
import { AcceptOfferResult, OpenOpportunity, Provider, ProviderInbox } from '../models/providers.models';
import { toParams } from '../http/params';

@Injectable({ providedIn: 'root' })
export class ProvidersApi {
  private http = inject(HttpClient);
  list(f: { type?: string; zone?: string; active?: boolean } = {}): Observable<Provider[]> { return this.http.get<Provider[]>('/api/providers', { params: toParams(f) }); }
  save(dto: Provider): Observable<Provider> { return dto.id ? this.http.put<Provider>(`/api/providers/${dto.id}`, dto) : this.http.post<Provider>('/api/providers', dto); }
  opportunities(branch?: number | null): Observable<OpenOpportunity[]> { return this.http.get<OpenOpportunity[]>('/api/providers/opportunities', { params: toParams({ branch }) }); }
  inbox(providerId: number): Observable<ProviderInbox> { return this.http.get<ProviderInbox>('/api/offers', { params: toParams({ providerId }) }); }
  accept(offerId: string, providerId?: number): Observable<AcceptOfferResult> { return this.http.post<AcceptOfferResult>(`/api/offers/${offerId}/accept`, {}, { params: toParams({ providerId }) }); }
}
