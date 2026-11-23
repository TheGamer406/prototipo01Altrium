/*
 * Archivo:    backend/src/Acompana.Domain/Catalog/Setting.cs
 * Qué hace:   Entidad Setting, se guarda en la tabla `settings`.
 * Recibe:     Campos: Key, Value (texto). Claves: ProviderAcceptTimeoutMin, TransferThresholdMin,
 *               DefaultMinStock. Los llena el handler que la crea o EF Core al leer.
 * Procesa:    Sin lógica; los valores iniciales salen de .env.
 * Devuelve:   Nada; es estado. Hereda Id, CreatedAt y UpdatedAt de Entity.
 * Se conecta: ← SettingsHandler, OfferTimeoutChecker, TransferDelayChecker
 * Reglas:     RF-17 · RF-24
 * Estado:     Pendiente
 */
