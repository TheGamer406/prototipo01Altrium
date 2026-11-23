/*
 * Archivo:    backend/src/Acompana.Application/Services/FamilyNotifier.cs
 * Qué hace:   Arma y envía los correos a la familia (no tiene usuario).
 * Recibe:     Service + tipo de aviso (Registrado, Factura emitida, En curso, Cenizas entregadas).
 * Procesa:    Plantilla en español con código, fecha, lugar y estado; adjunta referencia de factura si
 *               aplica.
 * Devuelve:   Task; queda en outbound_messages (Kind FamilyStatus o FamilyInvoice).
 * Se conecta: ← Create/IssueInvoice/RegisterPayment/AdvanceCremation · → IMailClient
 * Reglas:     RF-35
 * Estado:     Pendiente
 */
