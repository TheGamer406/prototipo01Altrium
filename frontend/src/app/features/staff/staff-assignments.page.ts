/**
 * Archivo:    frontend/src/app/features/staff/staff-assignments.page.ts
 * Qué hace:   Asignaciones de personal por turno.
 * Recibe:     Sede, turno.
 * Procesa:    Asignar empleado a servicio; aviso de traslape (no bloquea).
 * Devuelve:   Tabla por empleado.
 * Se conecta: → LogisticsApi
 * Reglas:     RF-25
 * Estado:     Completo
 */

import { Component, effect, inject, signal } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { MatSnackBar } from '@angular/material/snack-bar';
import { LogisticsApi } from '../../core/api/logistics.api';
import { ServicesApi } from '../../core/api/services.api';
import { PageHeaderComponent } from '../../core/layout/page-header.component';
import { StaffBoard } from '../../core/models/logistics.models';
import { ServiceListItem } from '../../core/models/services.models';
import { BranchContextService } from '../../core/state/branch-context.service';
import { CrDatePipe } from '../../shared/pipes/cr-date.pipe';

@Component({
  selector: 'app-staff-assignments-page',
  standalone: true,
  imports: [FormsModule, PageHeaderComponent, CrDatePipe],
  template: `
    <app-page-header title="Personal" description="Asigne personal a servicios pagados. Un traslape se permite pero genera una alerta." [rf]="['RF-25']" />
    <section class="card form"><h2>Nueva asignación</h2>
      <div class="row">
        <select [(ngModel)]="serviceId" aria-label="Servicio"><option value="">Servicio pagado…</option>@for (s of services(); track s.id) { <option [value]="s.id">{{ s.code }} · {{ s.deceasedName }}</option> }</select>
        <select [(ngModel)]="employeeId" aria-label="Empleado"><option [ngValue]="null">Empleado…</option>@for (e of board()?.employees ?? []; track e.id) { <option [ngValue]="e.id">{{ e.name }} · {{ roles[e.role] }} · {{ e.shift === 'Day' ? 'Día' : 'Noche' }}</option> }</select>
        <input type="datetime-local" [(ngModel)]="from" aria-label="Desde" /><input type="datetime-local" [(ngModel)]="to" aria-label="Hasta" />
        <button class="btn primary" type="button" [disabled]="!serviceId || !employeeId || !from || !to" (click)="assign()">Asignar</button></div></section>
    <h2>Asignaciones</h2>
    <table class="dense"><thead><tr><th>Servicio</th><th>Empleado</th><th>Desde</th><th>Hasta</th></tr></thead>
      <tbody>@for (a of board()?.assignments ?? []; track a.id) { <tr><td><strong>{{ a.serviceCode }}</strong></td><td>{{ a.employeeName }}</td><td>{{ a.from | crDate }}</td><td>{{ a.to | crDate }}</td></tr> }</tbody></table>`,
  styles: [`.form { margin-bottom: 20px; } .row { display: flex; gap: 8px; flex-wrap: wrap; margin-top: 8px; } .row select { min-width: 240px; } h2 { margin: 0 0 8px; }`],
})
export class StaffAssignmentsPage {
  private api = inject(LogisticsApi);
  private servicesApi = inject(ServicesApi);
  private snack = inject(MatSnackBar);
  private branch = inject(BranchContextService);
  readonly board = signal<StaffBoard | null>(null);
  readonly services = signal<ServiceListItem[]>([]);
  readonly roles: Record<string, string> = { Advisor: 'Asesor', BranchManager: 'Encargado', Driver: 'Chofer', CrematoriumOperator: 'Operador', Management: 'Gerencia' };
  serviceId = ''; employeeId: number | null = null; from = ''; to = '';
  constructor() { effect(() => { this.branch.branchId(); this.load(); }); }
  private load(): void {
    this.api.staff(this.branch.branchId()).subscribe(b => this.board.set(b));
    this.servicesApi.list({ branch: this.branch.branchId(), pageSize: 50, status: 'Paid' }).subscribe(p => this.services.set(p.items));
  }
  assign(): void {
    this.api.assignStaff({ serviceId: this.serviceId, employeeId: this.employeeId!, from: new Date(this.from).toISOString(), to: new Date(this.to).toISOString() }).subscribe(r => {
      this.snack.open(r.overlapWith ? `Asignado, pero se traslapa con ${r.overlapWith}. Se generó una alerta.` : 'Personal asignado.', 'Cerrar', { duration: 6000, panelClass: r.overlapWith ? 'error' : 'success' });
      this.load();
    });
  }
}
