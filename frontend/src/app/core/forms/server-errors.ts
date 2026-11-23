/**
 * Archivo:    frontend/src/app/core/forms/server-errors.ts
 * Qué hace:   Pinta errores del backend en los campos del formulario.
 * Recibe:     FormGroup + ProblemDetails.errors { campo: [mensajes] }.
 * Procesa:    setErrors({ server: mensaje }) en cada control que coincida (camelCase).
 * Devuelve:   void.
 * Se conecta: ← formularios · ← error.interceptor.ts
 * Estado:     Completo
 */

import { AbstractControl, FormGroup } from '@angular/forms';
import { ProblemDetails } from '../models/meta.models';

/** Pinta en cada control el primer mensaje del backend ('deceased.fullName' → form.deceased.fullName). */
export function applyServerErrors(form: FormGroup, problem: ProblemDetails | null | undefined): string[] {
  const unmatched: string[] = [];
  for (const [path, messages] of Object.entries(problem?.errors ?? {})) {
    const control: AbstractControl | null = form.get(path.split('.'));
    if (control) { control.setErrors({ server: messages[0] }); control.markAsTouched(); }
    else unmatched.push(messages[0]);
  }
  return unmatched;
}
