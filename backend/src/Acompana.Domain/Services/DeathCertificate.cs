/*
 * Archivo:    backend/src/Acompana.Domain/Services/DeathCertificate.cs
 * Qué hace:   Entidad DeathCertificate, se guarda en la tabla `death_certificates`.
 * Recibe:     Campos: ServiceId (único), Number, FilePath, RegisteredBy, RegisteredAt. Los llena el
 *               handler que la crea o EF Core al leer.
 * Procesa:    Sin lógica. Se usa el acta física; no se consulta al Registro Civil.
 * Devuelve:   Nada; es estado. Hereda Id, CreatedAt y UpdatedAt de Entity.
 * Se conecta: ← RegisterDeathCertificateHandler · → Service.ChangeStatus (requisito de Invoicing)
 * Reglas:     RF-03
 * Estado:     Pendiente
 */
