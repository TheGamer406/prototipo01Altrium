/**
 * Archivo:    frontend/src/app/core/layout/admin-menu.component.ts
 * Qué hace:   Menú Admin del demo.
 * Recibe:     AdminModeService.enabled().
 * Procesa:    Reiniciar demo (con confirmación), Refrescar analítica, Simular aceptación simultánea,
 *               Forzar rechazo de la próxima factura, Core contable caído 5 min.
 * Devuelve:   Llamadas a AdminApi + snackbar con el resultado.
 * Se conecta: ← topbar · → admin.api.ts
 * Reglas:     RF-13 · RF-16
 * Estado:     Completo
 */

import { Component, inject } from '@angular/core';
import { MatDialog } from '@angular/material/dialog';
import { MatMenuModule } from '@angular/material/menu';
import { MatSnackBar } from '@angular/material/snack-bar';
import { AdminApi } from '../api/admin.api';
import { SimulationResult } from '../models/admin.models';
import { ConfirmDialogComponent } from '../../shared/components/confirm-dialog.component';
import { Observable } from 'rxjs';

@Component({
  selector: 'app-admin-menu',
  standalone: true,
  imports: [MatMenuModule],
  template: `
    <button class="btn" type="button" [matMenuTriggerFor]="menu"><span class="ms">admin_panel_settings</span>Admin</button>
    <mat-menu #menu="matMenu">
      <button mat-menu-item type="button" (click)="reset()"><span class="ms">restart_alt</span> Reiniciar demo</button>
      <button mat-menu-item type="button" (click)="refresh()"><span class="ms">sync</span> Refrescar analítica</button>
      <button mat-menu-item type="button" (click)="simulate(api.simulateConcurrentAccept())"><span class="ms">bolt</span> Simular aceptación simultánea</button>
      <button mat-menu-item type="button" (click)="simulate(api.rejectNextInvoice())"><span class="ms">receipt_long</span> Forzar rechazo de la próxima factura</button>
      <button mat-menu-item type="button" (click)="simulate(api.coreDown(5))"><span class="ms">cloud_off</span> Core contable caído 5 min</button>
    </mat-menu>`,
  styles: [`.ms { margin-right: 8px; vertical-align: middle; }`],
})
export class AdminMenuComponent {
  readonly api = inject(AdminApi);
  private dialog = inject(MatDialog);
  private snack = inject(MatSnackBar);

  reset(): void {
    this.dialog.open(ConfirmDialogComponent, {
      data: { title: 'Reiniciar el demo', message: 'Se borrarán los datos y se volverá a sembrar la base. Tarda unos segundos.', confirmLabel: 'Reiniciar', danger: true },
    }).afterClosed().subscribe(ok => {
      if (!ok) return;
      this.snack.open('Reiniciando el demo…', undefined, { duration: 10000 });
      this.api.reset().subscribe({
        next: r => {
          try { sessionStorage.clear(); } catch { /* sin almacenamiento */ }
          this.snack.open(`Demo reiniciado: ${r.services} servicios en ${r.branches} sedes.`, 'Cerrar', { duration: 4000, panelClass: 'success' });
          setTimeout(() => window.location.assign('/'), 900);
        },
      });
    });
  }

  refresh(): void {
    this.api.refreshAnalytics().subscribe(() => this.snack.open('Analítica refrescada.', 'Cerrar', { duration: 3000, panelClass: 'success' }));
  }

  simulate(call: Observable<SimulationResult>): void {
    call.subscribe(r => this.snack.open([r.description, ...r.results].join(' · '), 'Cerrar', { duration: 8000, panelClass: 'success' }));
  }
}
