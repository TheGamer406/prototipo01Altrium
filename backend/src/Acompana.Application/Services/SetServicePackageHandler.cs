/*
 * Archivo:    backend/src/Acompana.Application/Services/SetServicePackageHandler.cs
 * Qué hace:   Cambia el paquete base y/o las líneas del servicio (personalización).
 * Recibe:     id + { basePackageId, lines[{componentId, casketModelId?, qty}] }.
 * Procesa:    1) solo si Status ∈ {Advance, Registered}
 *             2) reemplaza líneas copiando precio y exoneración del catálogo; el Package base no se
 *               toca
 *             3) IsCustomPackage = líneas ≠ ítems del paquete base (también si solo queda el cofre)
 *             4) RecalculateTotal
 * Devuelve:   ServiceDetailDto con líneas y total nuevos.
 * Se conecta: ← PUT /api/services/{id}/package · → AuditInterceptor (caso de las flores)
 * Reglas:     RF-04 · RF-05 · RF-10
 * Estado:     Pendiente
 *
 * Contrato:
 *   Task<ServiceDetailDto> HandleAsync(Guid id, SetPackageRequest req, CancellationToken ct)
 *     Recibe:     paquete base y líneas deseadas
 *     Procesa:    pasos 1-4
 *     Devuelve:   detalle
 *     Errores:    422 'El paquete ya no se puede cambiar: el servicio está facturado.'
 */
