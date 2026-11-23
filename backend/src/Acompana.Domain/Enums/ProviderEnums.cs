/*
 * Archivo:    backend/src/Acompana.Domain/Enums/ProviderEnums.cs
 * Qué hace:   Enumeraciones: OfferStatus (Open, Won, Lost, Expired).
 * Recibe:     —
 * Procesa:    Se guardan como texto en PostgreSQL (HasConversion<string>) para que pgAdmin y el grafo
 *               se lean claro.
 * Devuelve:   Valores que viajan en los DTO como texto; la interfaz los traduce al español.
 * Se conecta: ← entidades, DTO, configuraciones EF · → GET /api/meta/enums
 * Reglas:     RF-15 · RF-16 · RF-17
 * Estado:     Completo
 */

namespace Acompana.Domain.Enums;

public enum OfferStatus { Open, Won, Lost, Expired }
