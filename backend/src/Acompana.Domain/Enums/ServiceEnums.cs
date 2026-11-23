/*
 * Archivo:    backend/src/Acompana.Domain/Enums/ServiceEnums.cs
 * Qué hace:   Enumeraciones: ServiceKind (Immediate, Advance) · ServiceStatus (Advance, Registered,
 *               Invoicing, Paid, InProgress, Completed, Cancelled) · WakePlace (Chapel, Church, Home) ·
 *               FinalDestination (Cemetery, Cremation) · Payer (Family, Insurer) · LineStatus (Pending,
 *               Offered, Accepted, Delivered).
 * Recibe:     —
 * Procesa:    Se guardan como texto en PostgreSQL (HasConversion<string>) para que pgAdmin y el grafo
 *               se lean claro.
 * Devuelve:   Valores que viajan en los DTO como texto; la interfaz los traduce al español.
 * Se conecta: ← entidades, DTO, configuraciones EF · → GET /api/meta/enums
 * Reglas:     RF-01 · RF-02 · RF-07 · RF-11 · RF-19
 * Estado:     Pendiente
 */
