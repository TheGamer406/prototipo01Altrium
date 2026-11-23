/**
 * Archivo:    frontend/src/app/core/models/meta.models.ts
 * Qué hace:   Tipos TypeScript: Branch, RoleOption, EnumOption, PagedResult<T>, ProblemDetails.
 * Recibe:     —
 * Procesa:    Solo interfaces y tipos unión; deben coincidir con los DTO del backend.
 * Devuelve:   —
 * Se conecta: ← core/api/meta.api.ts y features
 * Estado:     Completo
 */

export interface Branch { id: number; code: string; name: string; zone: 'GAM' | 'Fuera'; }
export interface Option { value: string; label: string; }
export type RoleOption = Option;
export type EnumOption = Option;
export interface ProviderMeta { id: number; name: string; type: string; zone: string; }
export interface MetaEnums { [name: string]: Option[]; }
export interface PagedResult<T> { items: T[]; page: number; pageSize: number; total: number; }
export interface ProblemDetails { title: string; detail: string; status: number; errors?: Record<string, string[]> | null; }
