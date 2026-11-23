/*
 * Archivo:    backend/src/Acompana.Domain/Organization/FamilyContact.cs
 * Qué hace:   Entidad FamilyContact, se guarda en la tabla `family_contacts`.
 * Recibe:     Campos: FullName, NationalId, Relationship, Phone, Email, InsuranceProspectConsent
 *               (bool), ConsentAt?. Los llena el handler que la crea o EF Core al leer.
 * Procesa:    Si InsuranceProspectConsent pasa a true, ConsentAt se fija con la hora actual; si pasa a
 *               false, se borra.
 * Devuelve:   Nada; es estado. Hereda Id, CreatedAt y UpdatedAt de Entity.
 * Se conecta: ← Service.FamilyContactId · → ExportProspectsQuery, correos a la familia
 * Reglas:     RF-35 · RF-36 · RNF-14 (Ley 8968)
 * Estado:     Pendiente
 */
