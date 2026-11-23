/**
 * Archivo:    frontend/src/app/core/forms/unsaved-changes.guard.ts
 * Qué hace:   Guarda CanDeactivate para no perder cambios.
 * Recibe:     Componente que implementa HasUnsavedChanges { hasUnsavedChanges(): boolean }.
 * Procesa:    Si hay cambios, abre ConfirmDialog 'Tiene cambios sin guardar. ¿Salir de todos modos?'.
 * Devuelve:   Observable<boolean>.
 * Se conecta: ← rutas del asistente y formularios · → shared/components/confirm-dialog
 * Estado:     Pendiente
 */
