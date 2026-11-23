/**
 * Archivo:    frontend/src/app/shared/directives/submit-once.directive.ts
 * Qué hace:   Evita dobles envíos.
 * Recibe:     @Input submitOnce: Observable en curso.
 * Procesa:    Deshabilita el botón y muestra spinner hasta que el observable termina.
 * Devuelve:   —
 * Se conecta: ← botones de guardar/enviar
 * Estado:     Completo
 */

import { Directive, ElementRef, Input, OnChanges, inject } from '@angular/core';
import { Observable } from 'rxjs';
import { finalize } from 'rxjs/operators';

/** Uso: <button [submitOnce]="pending$">; deshabilita el botón mientras el observable no termine. */
@Directive({ selector: '[submitOnce]', standalone: true })
export class SubmitOnceDirective implements OnChanges {
  private el = inject<ElementRef<HTMLButtonElement>>(ElementRef);
  @Input() submitOnce: Observable<unknown> | null = null;
  private label = '';

  ngOnChanges(): void {
    const btn = this.el.nativeElement;
    if (!this.submitOnce) { btn.disabled = false; return; }
    this.label = btn.innerHTML;
    btn.disabled = true;
    btn.innerHTML = '<span class="ms" style="animation:spin 1s linear infinite">progress_activity</span>';
    this.submitOnce.pipe(finalize(() => { btn.disabled = false; btn.innerHTML = this.label; })).subscribe({ error: () => undefined });
  }
}
