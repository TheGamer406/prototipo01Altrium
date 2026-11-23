/**
 * Archivo:    frontend/src/app/features/services/new-service/step-casket.component.ts
 * Qué hace:   Paso 4: personalizar paquete y elegir cofre con imagen y stock de la sede.
 * Recibe:     Líneas actuales + catálogo filtrable (florales, catering y música con imagen) +
 *               CasketModelDto[] con stock.
 * Procesa:    Quitar/agregar componentes; cuadrícula de cofres (sin stock = deshabilitado con
 *               etiqueta); etiqueta 'Paquete personalizado' si difiere del base.
 * Devuelve:   Cambios en el FormGroup del asistente (no llama al API por su cuenta).
 * Se conecta: ← new-service-wizard · → CatalogApi / LogisticsApi (solo lectura)
 * Reglas:     RF-04 · RF-05 · RF-26
 * Estado:     Pendiente
 */
