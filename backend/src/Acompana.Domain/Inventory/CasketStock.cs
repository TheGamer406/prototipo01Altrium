/*
 * Archivo:    backend/src/Acompana.Domain/Inventory/CasketStock.cs
 * Qué hace:   Entidad CasketStock, se guarda en la tabla `casket_stock`.
 * Recibe:     Campos: BranchId, CasketModelId, Qty, MinQty, Version. Los llena el handler que la crea
 *               o EF Core al leer.
 * Procesa:    Deduct(qty): Qty -= qty; no baja de 0.
 *             IsAtOrBelowMin = Qty <= MinQty.
 * Devuelve:   Nada; es estado. Hereda Id, CreatedAt y UpdatedAt de Entity.
 * Se conecta: ← DeductCasketHandler · → StockQuery, alerta LowStock
 * Reglas:     RF-26 · RF-27
 * Estado:     Pendiente
 */
