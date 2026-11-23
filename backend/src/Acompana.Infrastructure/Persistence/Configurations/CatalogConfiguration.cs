/*
 * Archivo:    backend/src/Acompana.Infrastructure/Persistence/Configurations/CatalogConfiguration.cs
 * Qué hace:   Mapeo EF Core de las tablas: components, packages, package_items, casket_models,
 *               settings.
 * Recibe:     ModelBuilder (lo llama AppDbContext).
 * Procesa:    Nombres snake_case, enums como texto, decimales numeric(14,2), sku único.
 * Devuelve:   —
 * Se conecta: ← AppDbContext.OnModelCreating · → migraciones · → db/schema.sql
 * Estado:     Pendiente
 */
