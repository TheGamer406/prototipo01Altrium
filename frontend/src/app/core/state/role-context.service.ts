/**
 * Archivo:    frontend/src/app/core/state/role-context.service.ts
 * Qué hace:   Rol demo actual (signal) y permisos derivados.
 * Recibe:     Selección del usuario en la barra superior; valor inicial desde localStorage.
 * Procesa:    role = signal<DemoRole>('Advisor'); canSee(module) según la tabla de módulos por rol;
 *               persiste en localStorage.
 * Devuelve:   role(), roleLabel(), canSee(); lo leen menú, guardas e interceptor.
 * Se conecta: ← topbar · → nav-items.ts, demo-headers.interceptor.ts, roleGuard
 * Reglas:     RF-30
 * Estado:     Pendiente
 *
 * Contrato:
 *   setRole(role: DemoRole): void
 *     Recibe:     rol elegido
 *     Procesa:    actualiza la signal; si el rol no ve la ruta actual, navega a su inicio
 *     Devuelve:   void
 */
