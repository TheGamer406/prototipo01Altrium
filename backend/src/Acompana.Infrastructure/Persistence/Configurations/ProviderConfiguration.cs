/*
 * Archivo:    backend/src/Acompana.Infrastructure/Persistence/Configurations/ProviderConfiguration.cs
 * Qué hace:   Mapeo EF Core de las tablas: providers, provider_offers.
 * Recibe:     ModelBuilder (lo llama AppDbContext).
 * Procesa:    Nombres snake_case, enums como texto, decimales numeric(14,2), índice (service_line_id,
 *               status).
 * Devuelve:   —
 * Se conecta: ← AppDbContext.OnModelCreating · → migraciones · → db/schema.sql
 * Estado:     Pendiente
 */
