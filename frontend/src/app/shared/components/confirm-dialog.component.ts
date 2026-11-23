/**
 * Archivo:    frontend/src/app/shared/components/confirm-dialog.component.ts
 * Qué hace:   Diálogo de confirmación propio (no window.confirm).
 * Recibe:     MAT_DIALOG_DATA { title, message, confirmLabel, danger? }.
 * Procesa:    —
 * Devuelve:   afterClosed() → boolean.
 * Se conecta: ← unsaved-changes.guard, admin-menu, acciones destructivas
 * Estado:     Completo
 */

import { Component, inject } from '@angular/core';
import { MAT_DIALOG_DATA, MatDialogModule } from '@angular/material/dialog';

export interface ConfirmData { title: string; message: string; confirmLabel?: string; danger?: boolean; }

@Component({
  selector: 'app-confirm-dialog',
  standalone: true,
  imports: [MatDialogModule],
  template: `
    <h2 mat-dialog-title>{{ data.title }}</h2>
    <mat-dialog-content><p class="msg">{{ data.message }}</p></mat-dialog-content>
    <mat-dialog-actions align="end">
      <button class="btn ghost" type="button" [mat-dialog-close]="false">Cancelar</button>
      <button class="btn" [class.danger]="data.danger" [class.primary]="!data.danger" type="button" [mat-dialog-close]="true" cdkFocusInitial>{{ data.confirmLabel ?? 'Aceptar' }}</button>
    </mat-dialog-actions>`,
  styles: [`.msg { margin: 0; color: var(--text-muted); max-width: 420px; }`],
})
export class ConfirmDialogComponent { data = inject<ConfirmData>(MAT_DIALOG_DATA); }
