/**
 * Archivo:    frontend/src/app/features/services/new-service/step-wake-destination.component.ts
 * Qué hace:   Paso 5: velación y destino final.
 * Recibe:     FormGroup 'wake' + calendario de capillas.
 * Procesa:    Capilla (muestra disponibilidad) | iglesia | casa (pide dirección); cementerio (nombre)
 *               | cremación; fecha y hora.
 * Devuelve:   Cambios en el FormGroup del asistente (no llama al API por su cuenta).
 * Se conecta: ← new-service-wizard · → CatalogApi / LogisticsApi (solo lectura)
 * Reglas:     RF-07 · RF-20
 * Estado:     Pendiente
 */
