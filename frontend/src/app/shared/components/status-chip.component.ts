/**
 * Archivo:    frontend/src/app/shared/components/status-chip.component.ts
 * Qué hace:   Chip de estado (punto de color + texto, nunca solo color).
 * Recibe:     @Input status (cualquier enum) + kind ('service'|'line'|'invoice'|'offer'|'cremation').
 * Procesa:    Mapa estado → { label en español, token de color }.
 * Devuelve:   Chip pequeño.
 * Se conecta: ← listas, tableros, detalle
 * Estado:     Completo
 */

import { Component, Input } from '@angular/core';

type Kind = 'service' | 'line' | 'invoice' | 'offer' | 'cremation';
const MAP: Record<Kind, Record<string, [string, string]>> = {
  service: {
    Advance: ['Anticipado', 'info'], Registered: ['Registrado', 'muted'], Invoicing: ['En facturación', 'warning'], Paid: ['Pagado', 'accent'],
    InProgress: ['En curso', 'accent'], Completed: ['Finalizado', 'success'], Cancelled: ['Cancelado', 'danger'],
  },
  line: { Pending: ['Pendiente', 'muted'], Offered: ['Ofertado', 'warning'], Accepted: ['Aceptado', 'accent'], Delivered: ['Entregado', 'success'] },
  invoice: { Draft: ['Borrador', 'muted'], Sent: ['Enviada', 'warning'], Accepted: ['Aceptada', 'success'], Rejected: ['Rechazada', 'danger'] },
  offer: { Open: ['Abierta', 'warning'], Won: ['Ganada', 'success'], Lost: ['Perdida', 'muted'], Expired: ['Vencida', 'danger'] },
  cremation: { Waiting: ['En espera', 'muted'], InOven: ['En horno', 'warning'], Packed: ['Empacado', 'accent'], Delivered: ['Entregado', 'success'] },
};

@Component({
  selector: 'app-status-chip',
  standalone: true,
  template: `<span class="chip" [class]="'tone-' + tone"><i></i>{{ label }}</span>`,
  styles: [`.chip { display: inline-flex; align-items: center; gap: 6px; font-size: 12px; padding: 2px 8px; border: 1px solid var(--border); border-radius: 999px; white-space: nowrap; }
    i { width: 7px; height: 7px; border-radius: 50%; background: currentColor; }
    .tone-muted i { color: var(--text-muted); } .tone-info i { color: var(--info); } .tone-warning i { color: var(--warning); }
    .tone-accent i { color: var(--accent-text); } .tone-success i { color: var(--success); } .tone-danger i { color: var(--danger); }`],
})
export class StatusChipComponent {
  @Input() status = '';
  @Input() kind: Kind = 'service';
  get label(): string { return MAP[this.kind][this.status]?.[0] ?? this.status; }
  get tone(): string { return MAP[this.kind][this.status]?.[1] ?? 'muted'; }
}
