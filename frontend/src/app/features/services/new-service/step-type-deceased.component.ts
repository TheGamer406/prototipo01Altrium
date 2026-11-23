/**
 * Archivo:    frontend/src/app/features/services/new-service/step-type-deceased.component.ts
 * Qué hace:   Paso 1: tipo (inmediato/anticipado) y datos del fallecido.
 * Recibe:     FormGroup 'deceased' del asistente.
 * Procesa:    Valida cédula 1-0234-0567; fecha de defunción obligatoria si es inmediato.
 * Devuelve:   Cambios en el FormGroup del asistente (no llama al API por su cuenta).
 * Se conecta: ← new-service-wizard · → CatalogApi / LogisticsApi (solo lectura)
 * Reglas:     RF-01 · RF-02
 * Estado:     Pendiente
 */
