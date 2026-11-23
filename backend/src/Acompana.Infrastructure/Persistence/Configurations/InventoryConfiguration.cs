/*
 * Archivo:    backend/src/Acompana.Infrastructure/Persistence/Configurations/InventoryConfiguration.cs
 * Qué hace:   Mapeo EF Core de las tablas: casket_stock, restock_requests.
 * Recibe:     ModelBuilder (lo llama AppDbContext).
 * Procesa:    Nombres snake_case, enums como texto, decimales numeric(14,2), (branch_id,
 *               casket_model_id) único.
 * Devuelve:   —
 * Se conecta: ← AppDbContext.OnModelCreating · → migraciones · → db/schema.sql
 * Estado:     Pendiente
 */
