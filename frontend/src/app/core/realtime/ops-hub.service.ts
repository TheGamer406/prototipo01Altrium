/**
 * Archivo:    frontend/src/app/core/realtime/ops-hub.service.ts
 * Qué hace:   Conexión SignalR a /hubs/ops.
 * Recibe:     Sede y rol actuales (se reconecta al cambiar).
 * Procesa:    HubConnectionBuilder con reconexión automática; expone cada evento como Observable.
 * Devuelve:   serviceUpdated$, offerAwarded$, alertRaised$, cremationMoved$, stockChanged$.
 * Se conecta: → backend OpsHub · ← service-board, provider-inbox, central-view, cremation-board,
 *               topbar (alertas)
 * Reglas:     RF-19 · RF-29 · RNF-06
 * Estado:     Completo
 */

import { Injectable, effect, inject } from '@angular/core';
import * as signalR from '@microsoft/signalr';
import { Subject } from 'rxjs';
import { Alert } from '../models/oversight.models';
import { BranchContextService } from '../state/branch-context.service';
import { RoleContextService } from '../state/role-context.service';

@Injectable({ providedIn: 'root' })
export class OpsHubService {
  private role = inject(RoleContextService);
  private branch = inject(BranchContextService);
  private connection?: signalR.HubConnection;

  readonly serviceUpdated$ = new Subject<{ serviceId: string }>();
  readonly offerAwarded$ = new Subject<{ lineId: string; providerName: string; price: number; at: string }>();
  readonly alertRaised$ = new Subject<Alert>();
  readonly cremationMoved$ = new Subject<{ cremationId: number; status: string }>();
  readonly stockChanged$ = new Subject<{ branchId: number; modelId: number; qty: number }>();

  constructor() {
    // se reconecta al cambiar rol o sede
    effect(() => { this.connect(this.role.role(), this.branch.branchId()); });
  }

  private connect(role: string, branch: number | null): void {
    void this.connection?.stop();
    const qs = `?role=${role}` + (branch !== null ? `&branch=${branch}` : '');
    const c = new signalR.HubConnectionBuilder().withUrl(`/hubs/ops${qs}`).configureLogging(signalR.LogLevel.None).withAutomaticReconnect([0, 1000, 3000, 10000]).build();
    c.on('ServiceUpdated', p => this.serviceUpdated$.next(p));
    c.on('OfferAwarded', p => this.offerAwarded$.next(p));
    c.on('AlertRaised', p => this.alertRaised$.next(p));
    c.on('CremationMoved', p => this.cremationMoved$.next(p));
    c.on('StockChanged', p => this.stockChanged$.next(p));
    this.connection = c;
    c.start().catch(() => { /* reintenta el siguiente cambio de contexto; la app funciona sin tiempo real */ });
  }
}
