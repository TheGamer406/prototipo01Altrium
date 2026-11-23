/*
 * Archivo:    backend/src/Acompana.Application/Services/GetServiceDetailQuery.cs
 * Qué hace:   Detalle completo de un servicio.
 * Recibe:     id + ICurrentUser.
 * Procesa:    Include de líneas, acta, factura, familia; valida que el usuario vea la sede.
 * Devuelve:   ServiceDetailDto o 404 (también 404 si es de otra sede: no se revela que existe).
 * Se conecta: ← GET /api/services/{id}
 * Reglas:     RF-01 · RF-30
 * Estado:     Pendiente
 */
