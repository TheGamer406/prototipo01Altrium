/*
 * Archivo:    backend/src/Acompana.Domain/Enums/OversightEnums.cs
 * Qué hace:   Enumeraciones: AuditAction (Create, Update, Delete) · AlertType (ProviderTimeout,
 *               LowStock, TransferDelay, StaffOverlap, InvoiceRejected) · MessageChannel (Email) ·
 *               MessageKind (FamilyStatus, FamilyInvoice, InsurerNotice, ProviderOffer, Alert).
 * Recibe:     —
 * Procesa:    Se guardan como texto en PostgreSQL (HasConversion<string>) para que pgAdmin y el grafo
 *               se lean claro.
 * Devuelve:   Valores que viajan en los DTO como texto; la interfaz los traduce al español.
 * Se conecta: ← entidades, DTO, configuraciones EF · → GET /api/meta/enums
 * Reglas:     RF-31 · RF-35
 * Estado:     Pendiente
 */
