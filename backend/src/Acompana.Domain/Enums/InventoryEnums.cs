/*
 * Archivo:    backend/src/Acompana.Domain/Enums/InventoryEnums.cs
 * Qué hace:   Enumeraciones: RestockStatus (Requested, Dispatched, Received).
 * Recibe:     —
 * Procesa:    Se guardan como texto en PostgreSQL (HasConversion<string>) para que pgAdmin y el grafo
 *               se lean claro.
 * Devuelve:   Valores que viajan en los DTO como texto; la interfaz los traduce al español.
 * Se conecta: ← entidades, DTO, configuraciones EF · → GET /api/meta/enums
 * Reglas:     RF-28
 * Estado:     Pendiente
 */
