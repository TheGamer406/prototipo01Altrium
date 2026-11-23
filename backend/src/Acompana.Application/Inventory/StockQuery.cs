/*
 * Archivo:    backend/src/Acompana.Application/Inventory/StockQuery.cs
 * Qué hace:   Stock de cofres por sede y modelo.
 * Recibe:     branch? + ICurrentUser.
 * Procesa:    BranchScope; marca nivel ok / min / out.
 * Devuelve:   StockRowDto[].
 * Se conecta: ← GET /api/stock · paso 4 del asistente (cofre sin stock deshabilitado)
 * Reglas:     RF-26 · RF-27
 * Estado:     Pendiente
 */
