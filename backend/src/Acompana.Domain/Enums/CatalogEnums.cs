/*
 * Archivo:    backend/src/Acompana.Domain/Enums/CatalogEnums.cs
 * Qué hace:   Enumeraciones: ComponentCategory (Casket, Flowers, Hearse, Chapel, Church,
 *               BodyPreparation, Cremation, Catering, Music, Extra) · CasketOrigin (Nacional, Importado)
 *               · ProviderType (Flowers, Catering, Music).
 * Recibe:     —
 * Procesa:    Se guardan como texto en PostgreSQL (HasConversion<string>) para que pgAdmin y el grafo
 *               se lean claro.
 * Devuelve:   Valores que viajan en los DTO como texto; la interfaz los traduce al español.
 * Se conecta: ← entidades, DTO, configuraciones EF · → GET /api/meta/enums
 * Reglas:     RF-06 · RF-14
 * Estado:     Pendiente
 */
