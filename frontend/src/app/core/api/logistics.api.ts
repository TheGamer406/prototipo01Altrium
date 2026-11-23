/**
 * Archivo:    frontend/src/app/core/api/logistics.api.ts
 * Qué hace:   LogisticsApi: cliente HTTP tipado.
 * Recibe:     Parámetros tipados con core/models.
 * Procesa:    HttpClient contra /api/chapels, /api/chapel-bookings, /api/staff-assignments,
 *               /api/transfers, /api/cremations; sin lógica de negocio.
 * Devuelve:   Observables tipados: chapelCalendar(b, from, to), book(req), assignStaff(req),
 *               transfers(b), sendMilestones(id, batch), cremations(), advanceCremation(id, req).
 * Se conecta: ← features · → backend · ↔ core/models/logistics.models.ts
 * Reglas:     RF-20 a RF-25
 * Estado:     Parcial: capillas; personal, traslados y cremación en fases 5 y 6
 */

import { HttpClient } from '@angular/common/http';
import { Injectable, inject } from '@angular/core';
import { Observable } from 'rxjs';
import { BookChapelRequest, ChapelCalendar } from '../models/logistics.models';
import { toParams } from '../http/params';

@Injectable({ providedIn: 'root' })
export class LogisticsApi {
  private http = inject(HttpClient);
  chapelCalendar(branch: number | null, from: string, to: string): Observable<ChapelCalendar> { return this.http.get<ChapelCalendar>('/api/chapels/calendar', { params: toParams({ branch, from, to }) }); }
  book(req: BookChapelRequest): Observable<{ id: string }> { return this.http.post<{ id: string }>('/api/chapel-bookings', req); }
}
