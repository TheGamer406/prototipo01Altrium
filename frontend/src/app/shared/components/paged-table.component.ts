/**
 * Archivo:    frontend/src/app/shared/components/paged-table.component.ts
 * Qué hace:   Tabla densa paginada con estado en la URL.
 * Recibe:     @Input columns, data: PagedResult<T>, loading; query params de la ruta.
 * Procesa:    Orden y página se escriben en la URL (queryParamsHandling: 'merge') para que volver
 *               atrás los conserve.
 * Devuelve:   @Output pageChange, sortChange, rowClick.
 * Se conecta: ← listas (servicios, bitácora, mensajes)
 * Estado:     Pendiente
 */
