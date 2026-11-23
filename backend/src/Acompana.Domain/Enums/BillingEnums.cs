/*
 * Archivo:    backend/src/Acompana.Domain/Enums/BillingEnums.cs
 * Qué hace:   Enumeraciones: InvoiceStatus (Draft, Sent, Accepted, Rejected) · PaymentMethod (Card,
 *               Transfer, Cash, Insurer).
 * Recibe:     —
 * Procesa:    Se guardan como texto en PostgreSQL (HasConversion<string>) para que pgAdmin y el grafo
 *               se lean claro.
 * Devuelve:   Valores que viajan en los DTO como texto; la interfaz los traduce al español.
 * Se conecta: ← entidades, DTO, configuraciones EF · → GET /api/meta/enums
 * Reglas:     RF-08 · RF-09
 * Estado:     Completo
 */

namespace Acompana.Domain.Enums;

public enum InvoiceStatus { Draft, Sent, Accepted, Rejected }

public enum PaymentMethod { Card, Transfer, Cash, Insurer }
