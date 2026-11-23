/**
 * Archivo:    frontend/src/app/core/api/billing.api.ts
 * Qué hace:   BillingApi: cliente HTTP tipado.
 * Recibe:     Parámetros tipados con core/models.
 * Procesa:    HttpClient contra /api/services/{id}/invoice, /api/invoices/*; sin lógica de negocio.
 * Devuelve:   Observables tipados: issue(serviceId), pay(id, req), get(id).
 * Se conecta: ← features · → backend · ↔ core/models/billing.models.ts
 * Reglas:     RF-08 a RF-13
 * Estado:     Completo
 */

import { HttpClient } from '@angular/common/http';
import { Injectable, inject } from '@angular/core';
import { Observable } from 'rxjs';
import { Invoice, PayInvoiceRequest } from '../models/billing.models';

@Injectable({ providedIn: 'root' })
export class BillingApi {
  private http = inject(HttpClient);
  issue(serviceId: string): Observable<Invoice> { return this.http.post<Invoice>(`/api/services/${serviceId}/invoice`, {}); }
  pay(id: string, req: PayInvoiceRequest): Observable<Invoice> { return this.http.post<Invoice>(`/api/invoices/${id}/pay`, req); }
  get(id: string): Observable<Invoice> { return this.http.get<Invoice>(`/api/invoices/${id}`); }
}
