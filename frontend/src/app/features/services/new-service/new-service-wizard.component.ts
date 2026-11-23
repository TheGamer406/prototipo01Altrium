/**
 * Archivo:    frontend/src/app/features/services/new-service/new-service-wizard.component.ts
 * Qué hace:   Asistente 'Nuevo servicio' de 6 pasos.
 * Recibe:     Catálogo, stock y capillas de la sede actual.
 * Procesa:    FormGroup tipado con un subgrupo por paso; FormDraftService (clave 'nuevo-servicio'); se
 *               puede volver atrás sin perder datos; al terminar arma CreateServiceRequest, sube el acta
 *               y limpia el borrador.
 * Devuelve:   Servicio creado → navega al detalle en una pestaña ERP.
 * Se conecta: → step-*.component, total-panel, ServicesApi, FormDraftService, unsaved-changes.guard
 * Reglas:     RF-01 a RF-07 · RF-36 · RNF-11 (< 10 min)
 * Estado:     Pendiente
 *
 * Contrato:
 *   submit(): void
 *     Recibe:     formulario válido
 *     Procesa:    POST /api/services → POST death-certificate (si hay archivo) → clear borrador
 *     Devuelve:   navegación a /servicios/:id
 */
