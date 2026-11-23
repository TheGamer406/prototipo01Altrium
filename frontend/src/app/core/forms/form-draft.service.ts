/**
 * Archivo:    frontend/src/app/core/forms/form-draft.service.ts
 * Qué hace:   Guarda y restaura borradores de formularios.
 * Recibe:     FormGroup + clave (ruta + id).
 * Procesa:    valueChanges con debounceTime(1000) → sessionStorage; restore al iniciar; clear al
 *               guardar con éxito.
 * Devuelve:   Suscripción que el componente cancela al destruirse.
 * Se conecta: ← new-service-wizard y formularios de detalle/catálogo
 * Reglas:     —
 * Estado:     Pendiente
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
