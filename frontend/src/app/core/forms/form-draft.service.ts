/**
 * Archivo:    frontend/src/app/core/forms/form-draft.service.ts
 * Qué hace:   Guarda y restaura borradores de formularios.
 * Recibe:     FormGroup + clave (ruta + id).
 * Procesa:    valueChanges con debounceTime(1000) → sessionStorage; restore al iniciar; clear al
 *               guardar con éxito.
 * Devuelve:   Suscripción que el componente cancela al destruirse.
 * Se conecta: ← new-service-wizard y formularios de detalle/catálogo
 * Reglas:     —
 * Estado:     Completo
 *
 * Contrato:
 *   attach(form: FormGroup, key: string): Subscription
 *     Recibe:     formulario y clave
 *     Procesa:    restaura si hay borrador y empieza a guardar
 *     Devuelve:   suscripción
 *   clear(key: string): void
 *     Recibe:     clave
 *     Procesa:    borra de sessionStorage
 *     Devuelve:   void
 */

import { Injectable } from '@angular/core';
import { FormGroup } from '@angular/forms';
import { Subscription, debounceTime } from 'rxjs';

@Injectable({ providedIn: 'root' })
export class FormDraftService {
  attach(form: FormGroup, key: string): Subscription {
    try {
      const raw = sessionStorage.getItem(`acompana.draft.${key}`);
      if (raw) form.patchValue(JSON.parse(raw), { emitEvent: false });
    } catch { /* borrador dañado: se ignora */ }
    return form.valueChanges.pipe(debounceTime(1000)).subscribe(v => {
      try { sessionStorage.setItem(`acompana.draft.${key}`, JSON.stringify(v)); } catch { /* sin almacenamiento */ }
    });
  }

  has(key: string): boolean {
    try { return sessionStorage.getItem(`acompana.draft.${key}`) !== null; } catch { return false; }
  }

  clear(key: string): void {
    try { sessionStorage.removeItem(`acompana.draft.${key}`); } catch { /* sin almacenamiento */ }
  }
}
