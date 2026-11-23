/**
 * Archivo:    frontend/src/app/core/api/inventory.api.ts
 * Qué hace:   InventoryApi: cliente HTTP tipado.
 * Recibe:     Parámetros tipados con core/models.
 * Procesa:    HttpClient contra /api/stock, /api/restock-requests; sin lógica de negocio.
 * Devuelve:   Observables tipados: stock(b), restockRequests(f), dispatch(id).
 * Se conecta: ← features · → backend · ↔ core/models/inventory.models.ts
 * Reglas:     RF-26 a RF-28
 * Estado:     Completo
 */

import { HttpClient } from '@angular/common/http';
import { Injectable, inject } from '@angular/core';
import { Observable } from 'rxjs';
import { RestockRequest, StockRow } from '../models/inventory.models';
import { toParams } from '../http/params';

@Injectable({ providedIn: 'root' })
export class InventoryApi {
  private http = inject(HttpClient);
  stock(branch?: number | null): Observable<StockRow[]> { return this.http.get<StockRow[]>('/api/stock', { params: toParams({ branch }) }); }
  restockRequests(f: { status?: string; branch?: number | null } = {}): Observable<RestockRequest[]> { return this.http.get<RestockRequest[]>('/api/restock-requests', { params: toParams(f) }); }
  dispatch(id: number): Observable<RestockRequest> { return this.http.post<RestockRequest>(`/api/restock-requests/${id}/dispatch`, {}); }
}
