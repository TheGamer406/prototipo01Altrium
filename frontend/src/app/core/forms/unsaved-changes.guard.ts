/**
 * Archivo:    frontend/src/app/core/forms/unsaved-changes.guard.ts
 * Qué hace:   Guarda CanDeactivate para no perder cambios.
 * Recibe:     Componente que implementa HasUnsavedChanges { hasUnsavedChanges(): boolean }.
 * Procesa:    Si hay cambios, abre ConfirmDialog 'Tiene cambios sin guardar. ¿Salir de todos modos?'.
 * Devuelve:   Observable<boolean>.
 * Se conecta: ← rutas del asistente y formularios · → shared/components/confirm-dialog
 * Estado:     Completo
 */

import { inject } from '@angular/core';
import { CanDeactivateFn } from '@angular/router';
import { MatDialog } from '@angular/material/dialog';
import { Observable, of } from 'rxjs';
import { map } from 'rxjs/operators';
import { ConfirmDialogComponent } from '../../shared/components/confirm-dialog.component';

export interface HasUnsavedChanges { hasUnsavedChanges(): boolean; }

export const unsavedChangesGuard: CanDeactivateFn<HasUnsavedChanges> = (component): Observable<boolean> => {
  if (!component.hasUnsavedChanges()) return of(true);
  return inject(MatDialog).open(ConfirmDialogComponent, {
    data: { title: 'Cambios sin guardar', message: 'Tiene cambios sin guardar. ¿Salir de todos modos?', confirmLabel: 'Salir', danger: true },
  }).afterClosed().pipe(map(v => v === true));
};
