/*
 * Archivo:    backend/src/Acompana.Application/Services/CreateServiceHandler.cs
 * Qué hace:   Registra un servicio nuevo (inmediato o anticipado) desde el asistente.
 * Recibe:     CreateServiceRequest + ICurrentUser.
 * Procesa:    1) valida campos (FluentValidation-like a mano, mensajes en español)
 *             2) crea Deceased y FamilyContact (ConsentAt si hay consentimiento)
 *             3) crea Service con Code consecutivo SRV-{año}-{00000}, Status = Advance o Registered
 *             4) copia líneas: precio y % exento desde el catálogo; IsCustomPackage si difieren del
 *               paquete base
 *             5) si WakePlace = Chapel reserva la capilla (BookChapelHandler)
 *             6) RecalculateTotal y guarda en una transacción; notifica ServiceUpdated
 * Devuelve:   ServiceDetailDto del servicio creado (201 Created + Location).
 * Se conecta: ← ServicesController POST /api/services · → AppDbContext, BookChapelHandler,
 *               IOpsNotifier, FamilyNotifier (correo de registro)
 * Reglas:     RF-01 · RF-02 · RF-04 · RF-05 · RF-07 · RF-36
 * Estado:     Pendiente
 *
 * Contrato:
 *   Task<ServiceDetailDto> HandleAsync(CreateServiceRequest req, CancellationToken ct)
 *     Recibe:     la solicitud completa del asistente (6 pasos)
 *     Procesa:    pasos 1-6
 *     Devuelve:   detalle del servicio
 *     Errores:    400 validación por campo · 409 capilla ocupada (mensaje con la reserva que choca)
 */
