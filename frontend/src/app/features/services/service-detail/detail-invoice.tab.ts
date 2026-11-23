/**
 * Archivo:    frontend/src/app/features/services/service-detail/detail-invoice.tab.ts
 * Qué hace:   Factura y pago.
 * Recibe:     Detalle + factura.
 * Procesa:    'Enviar al core contable' → 'Enviando…' → aceptada (número del core) o rechazada (motivo
 *               + corregir y reenviar); 'Registrar pago'.
 * Devuelve:   Vista de la pestaña; acciones vía el Api correspondiente.
 * Se conecta: ← service-detail.page
 * Reglas:     RF-08 a RF-13
 * Estado:     Completo
 */

import { Component, EventEmitter, Input, Output, inject, signal } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { MatSnackBar } from '@angular/material/snack-bar';
import { BillingApi } from '../../../core/api/billing.api';
import { PaymentMethod } from '../../../core/models/billing.models';
import { ServiceDetail } from '../../../core/models/services.models';
import { StatusChipComponent } from '../../../shared/components/status-chip.component';
import { ColonesPipe } from '../../../shared/pipes/colones.pipe';
import { CrDatePipe } from '../../../shared/pipes/cr-date.pipe';

@Component({
  selector: 'app-detail-invoice-tab',
  standalone: true,
  imports: [FormsModule, StatusChipComponent, ColonesPipe, CrDatePipe],
  template: `
    <div class="card">
      @if (s.invoice; as i) {
        <header><h2>Factura {{ i.coreReference ?? i.number }}</h2><app-status-chip [status]="i.status" kind="invoice" />@if (i.paid) { <span class="paid">Pagada {{ i.paidAt | crDate }}</span> }</header>
        <p class="muted">Receptor: {{ i.receiverName }} ({{ i.receiverNationalId }})</p>
        <table class="dense"><thead><tr><th>Detalle</th><th>Cant.</th><th class="num">Precio</th><th>Exento</th><th class="num">IVA</th><th class="num">Total línea</th></tr></thead>
          <tbody>@for (l of i.lines; track $index) { <tr><td>{{ l.description }}</td><td>{{ l.qty }}</td><td class="num">{{ l.unitPriceCrc | colones }}</td><td>{{ l.taxExemptPct }} %</td><td class="num">{{ l.taxCrc | colones }}</td><td class="num">{{ l.lineTotalCrc | colones }}</td></tr> }</tbody></table>
        <dl><dt>Subtotal</dt><dd>{{ i.subtotalCrc | colones }}</dd><dt>Exonerado</dt><dd>{{ i.exemptCrc | colones }}</dd><dt>IVA</dt><dd>{{ i.taxCrc | colones }}</dd><dt class="big">Total</dt><dd class="big">{{ i.totalCrc | colones }}</dd></dl>
        @if (i.status === 'Rejected') { <p class="err">Rechazada por el core contable: {{ i.rejectReason }}. Corrija los datos de la familia y vuelva a enviar.</p> }
      } @else { <p class="muted">Aún no se ha emitido la factura. La emite el core contable ante Hacienda.</p> }

      @if (canIssue) { <button class="btn primary" type="button" [disabled]="busy()" (click)="issue()">{{ busy() ? 'Enviando al core contable…' : (s.invoice ? 'Reenviar al core contable' : 'Enviar al core contable') }}</button> }
      @if (canPay) {
        <div class="pay"><select [(ngModel)]="method" aria-label="Método de pago"><option value="Card">Tarjeta</option><option value="Transfer">Transferencia</option><option value="Cash">Efectivo</option><option value="Insurer">Aseguradora</option></select>
          <button class="btn primary" type="button" [disabled]="busy()" (click)="pay()">Registrar pago de {{ s.invoice!.totalCrc | colones }}</button></div> }
    </div>`,
  styles: [`header { display: flex; align-items: center; gap: 12px; } h2 { margin: 0; } .paid { color: var(--success); } dl { display: grid; grid-template-columns: 1fr auto; gap: 4px 16px; max-width: 320px; margin: 12px 0 12px auto; } dd { margin: 0; text-align: right; }
    .big { font-size: 16px; font-weight: 600; } .err { color: #e0a0a0; } .pay { display: flex; gap: 8px; margin-top: 8px; }`],
})
export class DetailInvoiceTab {
  private api = inject(BillingApi);
  private snack = inject(MatSnackBar);
  @Input({ required: true }) s!: ServiceDetail;
  @Output() changed = new EventEmitter<void>();
  method: PaymentMethod = 'Card';
  readonly busy = signal(false);
  get canIssue(): boolean { return (this.s.status === 'Registered' || this.s.status === 'Invoicing') && this.s.invoice?.status !== 'Accepted'; }
  get canPay(): boolean { return this.s.invoice?.status === 'Accepted' && !this.s.invoice.paid; }
  issue(): void {
    this.busy.set(true);
    this.api.issue(this.s.id).subscribe({
      next: i => { this.busy.set(false); this.snack.open(i.status === 'Accepted' ? `Factura ${i.coreReference} aceptada.` : `Factura rechazada: ${i.rejectReason}`, 'Cerrar', { duration: 6000, panelClass: i.status === 'Accepted' ? 'success' : 'error' }); this.changed.emit(); },
      error: () => { this.busy.set(false); this.changed.emit(); },
    });
  }
  pay(): void {
    this.busy.set(true);
    this.api.pay(this.s.invoice!.id, { method: this.method, amountCrc: this.s.invoice!.totalCrc }).subscribe({
      next: () => { this.busy.set(false); this.snack.open('Pago registrado. Se enviaron las ofertas a los proveedores.', 'Cerrar', { duration: 5000, panelClass: 'success' }); this.changed.emit(); },
      error: () => this.busy.set(false),
    });
  }
}
