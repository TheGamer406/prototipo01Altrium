/**
 * Archivo:    frontend/src/app/core/http/error.interceptor.ts
 * Qué hace:   Manejo uniforme de errores HTTP.
 * Recibe:     HttpErrorResponse.
 * Procesa:    Error de red (status 0) → reintenta una vez; ProblemDetails → snackbar con detail; 400
 *               con errors → deja el error para que el formulario lo pinte por campo.
 * Devuelve:   Observable de error re-lanzado (el componente decide).
 * Se conecta: ← app.config.ts · → MatSnackBar, server-errors.ts
 * Estado:     Completo
 */

import { HttpErrorResponse, HttpInterceptorFn } from '@angular/common/http';
import { inject } from '@angular/core';
import { MatSnackBar } from '@angular/material/snack-bar';
import { catchError, retry, throwError, timer } from 'rxjs';
import { ProblemDetails } from '../models/meta.models';

export const errors: HttpInterceptorFn = (req, next) => {
  const snack = inject(MatSnackBar);
  return next(req).pipe(
    // un fallo de red se reintenta una vez (solo lecturas)
    retry({ count: req.method === 'GET' ? 1 : 0, delay: (e: HttpErrorResponse) => (e.status === 0 ? timer(600) : throwError(() => e)) }),
    catchError((e: HttpErrorResponse) => {
      const problem = e.error as ProblemDetails | null;
      const byField = e.status === 400 && problem?.errors && Object.keys(problem.errors).length > 0;
      if (!byField) {
        const text = e.status === 0 ? 'No hay conexión con el servidor.' : problem?.detail ?? 'Ocurrió un error inesperado.';
        snack.open(text, 'Cerrar', { duration: 6000, panelClass: 'error' });
      }
      return throwError(() => e);
    }),
  );
};
