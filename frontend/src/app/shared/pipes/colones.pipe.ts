/**
 * Archivo:    frontend/src/app/shared/pipes/colones.pipe.ts
 * Qué hace:   Formato de colones.
 * Recibe:     number (decimal).
 * Procesa:    Redondea a entero; separador de miles con espacio fino; prefijo ₡.
 * Devuelve:   '₡1 450 000'.
 * Se conecta: ← todas las vistas con montos
 * Estado:     Completo
 */

import { Pipe, PipeTransform } from '@angular/core';

@Pipe({ name: 'colones', standalone: true })
export class ColonesPipe implements PipeTransform {
  transform(value: number | null | undefined): string {
    if (value === null || value === undefined || Number.isNaN(value)) return '—';
    const n = Math.round(value);
    const digits = Math.abs(n).toString().replace(/\B(?=(\d{3})+(?!\d))/g, ' ');
    return `${n < 0 ? '-' : ''}₡${digits}`;
  }
}
