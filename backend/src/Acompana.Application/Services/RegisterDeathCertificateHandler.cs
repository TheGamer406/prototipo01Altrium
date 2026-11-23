/*
 * Archivo:    backend/src/Acompana.Application/Services/RegisterDeathCertificateHandler.cs
 * Qué hace:   Registra el acta de defunción (número + archivo) del servicio.
 * Recibe:     id + multipart { number, file (PDF/JPG/PNG ≤ 5 MB) }.
 * Procesa:    Guarda el archivo en el volumen /data/uploads/{serviceId}/; crea o reemplaza
 *               DeathCertificate.
 * Devuelve:   DeathCertificateDto { number, fileUrl, registeredBy, registeredAt }.
 * Se conecta: ← POST /api/services/{id}/death-certificate · → habilita Invoicing
 *               (Service.ChangeStatus)
 * Reglas:     RF-03
 * Estado:     Pendiente
 *
 * Contrato:
 *   Task<DeathCertificateDto> HandleAsync(Guid id, string number, IFormFile file, CancellationToken
 *       ct)
 *     Recibe:     id, número de acta, archivo
 *     Procesa:    ver arriba
 *     Devuelve:   datos del acta
 *     Errores:    400 'Formato no permitido (PDF, JPG o PNG).' · 400 'Archivo mayor a 5 MB.'
 */
