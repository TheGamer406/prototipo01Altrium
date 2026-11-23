/*
 * Archivo:    backend/src/Acompana.Domain/Catalog/CasketModel.cs
 * Qué hace:   Entidad CasketModel, se guarda en la tabla `casket_models`.
 * Recibe:     Campos: Sku, Name, Material, Origin (Nacional/Importado), PriceCrc, ImagePath
 *               (img/cofres/cofreN.webp). Los llena el handler que la crea o EF Core al leer.
 * Procesa:    Sin lógica. 10 modelos: 6 nacionales y 4 importados.
 * Devuelve:   Nada; es estado. Hereda Id, CreatedAt y UpdatedAt de Entity.
 * Se conecta: ← ServiceLine.CasketModelId, CasketStock
 * Reglas:     RF-04 · RF-26
 * Estado:     Pendiente
 */
