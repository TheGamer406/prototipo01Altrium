/**
 * Archivo:    frontend/src/app/core/http/params.ts
 * Qué hace:   Arma HttpParams omitiendo valores vacíos.
 * Recibe:     Objeto { clave: valor | null | undefined }.
 * Procesa:    Ignora null, undefined y cadenas vacías.
 * Devuelve:   HttpParams.
 * Se conecta: ← core/api/*
 * Estado:     Completo
 */
import { HttpParams } from '@angular/common/http';

export function toParams(obj: Record<string, string | number | boolean | null | undefined>): HttpParams {
  let p = new HttpParams();
  for (const [k, v] of Object.entries(obj)) {
    if (v !== null && v !== undefined && v !== '') p = p.set(k, String(v));
  }
  return p;
}
