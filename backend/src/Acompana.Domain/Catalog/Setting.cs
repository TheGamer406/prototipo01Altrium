/*
 * Archivo:    backend/src/Acompana.Domain/Catalog/Setting.cs
 * Qué hace:   Entidad Setting, se guarda en la tabla `settings`.
 * Recibe:     Campos: Key, Value (texto). Claves: ProviderAcceptTimeoutMin, TransferThresholdMin,
 *               DefaultMinStock. Los llena el handler que la crea o EF Core al leer.
 * Procesa:    Sin lógica; los valores iniciales salen de .env.
 * Devuelve:   Nada; es estado. Hereda Id, CreatedAt y UpdatedAt de Entity.
 * Se conecta: ← SettingsHandler, OfferTimeoutChecker, TransferDelayChecker
 * Reglas:     RF-17 · RF-24
 * Estado:     Completo
 */

using Acompana.Domain.Common;

namespace Acompana.Domain.Catalog;

public class Setting : Entity
{
    public string Key { get; set; } = "";
    public string Value { get; set; } = "";

    public const string ProviderAcceptTimeoutMin = "ProviderAcceptTimeoutMin";
    public const string TransferThresholdMin = "TransferThresholdMin";
    public const string DefaultMinStock = "DefaultMinStock";
}
