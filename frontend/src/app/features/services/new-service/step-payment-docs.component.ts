/**
 * Archivo:    frontend/src/app/features/services/new-service/step-payment-docs.component.ts
 * Qué hace:   Paso 6: pagador, acta y resumen.
 * Recibe:     FormGroup 'payment' + totales.
 * Procesa:    Familia | aseguradora (póliza); acta: número + archivo PDF/JPG/PNG; resumen con desglose
 *               de IVA y exoneraciones.
 * Devuelve:   Cambios en el FormGroup del asistente (no llama al API por su cuenta).
 * Se conecta: ← new-service-wizard · → CatalogApi / LogisticsApi (solo lectura)
 * Reglas:     RF-03 · RF-10 · RF-11
 * Estado:     Pendiente
 */
