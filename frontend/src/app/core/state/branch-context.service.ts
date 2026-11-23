/**
 * Archivo:    frontend/src/app/core/state/branch-context.service.ts
 * Qué hace:   Sede actual (signal).
 * Recibe:     Selección en la barra superior; lista de sedes de GET /api/meta/branches.
 * Procesa:    branchId = signal<number|null>; null solo permitido para Gerencia ('Todas').
 * Devuelve:   branchId(), branchName(), branches().
 * Se conecta: ← topbar · → demo-headers.interceptor.ts, ops-hub.service.ts
 * Reglas:     RF-30
 * Estado:     Pendiente
 */
