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
 * Estado:     Completo
 */

using Acompana.Domain.Common;

namespace Acompana.Domain.Organization;

public class FamilyContact : Entity
{
    public string FullName { get; set; } = "";
    public string NationalId { get; set; } = "";
    public string Relationship { get; set; } = "";
    public string Phone { get; set; } = "";
    public string Email { get; set; } = "";
    public bool InsuranceProspectConsent { get; private set; }
    public DateTimeOffset? ConsentAt { get; private set; }

    /// <summary>RF-35 / RNF-14: fija o borra la fecha del consentimiento según el valor.</summary>
    public void SetConsent(bool consent, DateTimeOffset now)
    {
        if (consent && !InsuranceProspectConsent) ConsentAt = now;
        if (!consent) ConsentAt = null;
        InsuranceProspectConsent = consent;
    }
}
