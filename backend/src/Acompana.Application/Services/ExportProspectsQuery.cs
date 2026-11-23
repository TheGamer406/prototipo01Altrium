/*
 * Archivo:    backend/src/Acompana.Application/Services/ExportProspectsQuery.cs
 * Qué hace:   Exporta familiares como prospectos de seguro, solo con consentimiento.
 * Recibe:     from, to (fechas) + rol Management.
 * Procesa:    FamilyContacts con InsuranceProspectConsent = true y ConsentAt en el rango.
 * Devuelve:   CSV (text/csv; UTF-8 con BOM) con nombre, teléfono, correo, fecha de consentimiento.
 * Se conecta: ← GET /api/prospects/export
 * Reglas:     RF-36 · RNF-14
 * Estado:     Pendiente
 */
