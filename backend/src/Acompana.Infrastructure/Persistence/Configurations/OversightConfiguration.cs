/*
 * Archivo:    backend/src/Acompana.Infrastructure/Persistence/Configurations/OversightConfiguration.cs
 * Qué hace:   Mapeo EF Core de las tablas: audit_log, alerts, outbound_messages.
 * Recibe:     ModelBuilder (lo llama AppDbContext).
 * Procesa:    Nombres snake_case, enums como texto, decimales numeric(14,2), jsonb para old/new
 *               values.
 * Devuelve:   —
 * Se conecta: ← AppDbContext.OnModelCreating · → migraciones · → db/schema.sql
 * Estado:     Pendiente
 */
