/**
 * Archivo:    frontend/src/app/features/driver-app/offline-queue.service.ts
 * Qué hace:   Cola de hitos sin señal.
 * Recibe:     Hito con clientUuid (crypto.randomUUID()).
 * Procesa:    Guarda en localStorage; al volver la señal envía el lote; reintenta hasta 200.
 * Devuelve:   Signal con pendientes.
 * Se conecta: ← driver-app.page · → LogisticsApi.sendMilestones
 * Reglas:     RF-22 · RNF-05
 * Estado:     Completo
 */

import { Injectable, inject, signal } from '@angular/core';
import { MatSnackBar } from '@angular/material/snack-bar';
import { LogisticsApi } from '../../core/api/logistics.api';
import { Milestone } from '../../core/models/logistics.models';

const KEY = 'acompana.milestone-queue';
const MAX = 200;
interface Queued { transferId: number; item: Milestone; }

@Injectable({ providedIn: 'root' })
export class OfflineQueueService {
  private api = inject(LogisticsApi);
  private snack = inject(MatSnackBar);
  /** Simula la falta de señal del teléfono. */
  readonly offline = signal(false);
  readonly pending = signal<Queued[]>(this.read());

  private read(): Queued[] { try { return JSON.parse(localStorage.getItem(KEY) ?? '[]'); } catch { return []; } }
  private write(): void { try { localStorage.setItem(KEY, JSON.stringify(this.pending())); } catch { /* sin almacenamiento */ } }

  /** Registra un hito: sin señal se encola con su hora real; con señal se envía de inmediato. */
  record(transferId: number, type: string, lat: number, lng: number, onSent: () => void): void {
    const item: Milestone = { clientUuid: crypto.randomUUID(), type, recordedAt: new Date().toISOString(), lat, lng, source: this.offline() ? 'Offline' : 'Online' };
    if (this.offline()) {
      this.pending.update(p => [...p, { transferId, item }].slice(-MAX)); this.write();
      this.snack.open('Sin señal: el hito quedó en cola y se enviará al volver la conexión.', 'Cerrar', { duration: 4000 });
      return;
    }
    this.api.sendMilestones(transferId, { items: [item] }).subscribe({
      next: () => onSent(),
      error: () => { this.pending.update(p => [...p, { transferId, item: { ...item, source: 'Offline' } }]); this.write(); },
    });
  }

  setOffline(value: boolean, onSynced: () => void): void {
    this.offline.set(value);
    if (!value) this.flush(onSynced);
  }

  flush(onSynced: () => void): void {
    const queue = this.pending();
    if (!queue.length) return;
    const byTransfer = new Map<number, Milestone[]>();
    for (const q of queue) byTransfer.set(q.transferId, [...(byTransfer.get(q.transferId) ?? []), q.item]);
    let left = byTransfer.size; let dups = 0; let ok = 0;
    byTransfer.forEach((items, id) => this.api.sendMilestones(id, { items }).subscribe({
      next: r => {
        ok += r.accepted; dups += r.duplicates;
        this.pending.update(p => p.filter(x => x.transferId !== id)); this.write();
        if (--left === 0) { this.snack.open(`Señal recuperada: ${ok} hito(s) sincronizados${dups ? `, ${dups} ya estaban` : ''}.`, 'Cerrar', { duration: 5000, panelClass: 'success' }); onSynced(); }
      },
      error: () => { left--; },
    }));
  }
}
