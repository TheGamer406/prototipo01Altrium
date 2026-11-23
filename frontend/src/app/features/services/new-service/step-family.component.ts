/**
 * Archivo:    frontend/src/app/features/services/new-service/step-family.component.ts
 * Qué hace:   Paso 2: familia responsable y consentimiento.
 * Recibe:     FormGroup 'family'.
 * Procesa:    Correo obligatorio (la familia recibe avisos por correo); casilla de consentimiento con
 *               texto de la Ley 8968.
 * Devuelve:   Cambios en el FormGroup del asistente (no llama al API por su cuenta).
 * Se conecta: ← new-service-wizard · → CatalogApi / LogisticsApi (solo lectura)
 * Reglas:     RF-35 · RF-36 · RNF-14
 * Estado:     Pendiente
 */
