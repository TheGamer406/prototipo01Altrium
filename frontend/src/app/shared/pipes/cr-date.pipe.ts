/**
 * Archivo:    frontend/src/app/shared/pipes/cr-date.pipe.ts
 * Qué hace:   Formato de fecha y hora de Costa Rica.
 * Recibe:     string ISO o Date + formato ('date' | 'datetime' | 'time').
 * Procesa:    Intl.DateTimeFormat('es-CR', { timeZone: 'America/Costa_Rica', hour12: false })
 * Devuelve:   '25/11/2026', '25/11/2026 14:32' o '14:32'.
 * Se conecta: ← todas las vistas con fechas
 * Estado:     Completo
 */

import { Pipe, PipeTransform } from '@angular/core';

const TZ = 'America/Costa_Rica';

@Pipe({ name: 'crDate', standalone: true })
export class CrDatePipe implements PipeTransform {
  transform(value: string | Date | null | undefined, format: 'date' | 'datetime' | 'time' = 'datetime'): string {
    if (!value) return '—';
    const d = typeof value === 'string' ? new Date(value) : value;
    if (Number.isNaN(d.getTime())) return '—';
    const date = new Intl.DateTimeFormat('es-CR', { timeZone: TZ, day: '2-digit', month: '2-digit', year: 'numeric' }).format(d);
    const time = new Intl.DateTimeFormat('es-CR', { timeZone: TZ, hour: '2-digit', minute: '2-digit', hour12: false }).format(d);
    return format === 'date' ? date : format === 'time' ? time : `${date} ${time}`;
  }
}
