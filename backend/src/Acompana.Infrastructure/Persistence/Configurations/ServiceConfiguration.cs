/*
 * Archivo:    backend/src/Acompana.Infrastructure/Persistence/Configurations/ServiceConfiguration.cs
 * Qué hace:   Mapeo EF Core de las tablas: services, service_lines, death_certificates,
 *               status_history.
 * Recibe:     ModelBuilder (lo llama AppDbContext).
 * Procesa:    Nombres snake_case, enums como texto, decimales numeric(14,2), code único;
 *               death_certificates.service_id único; índices (branch_id, status).
 * Devuelve:   —
 * Se conecta: ← AppDbContext.OnModelCreating · → migraciones · → db/schema.sql
 * Estado:     Pendiente
 */
