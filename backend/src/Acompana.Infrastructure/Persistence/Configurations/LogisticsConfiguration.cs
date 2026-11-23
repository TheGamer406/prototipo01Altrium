/*
 * Archivo:    backend/src/Acompana.Infrastructure/Persistence/Configurations/LogisticsConfiguration.cs
 * Qué hace:   Mapeo EF Core de las tablas: chapels, chapel_bookings, staff_assignments, hearses,
 *               transfers, transfer_milestones, cremations.
 * Recibe:     ModelBuilder (lo llama AppDbContext).
 * Procesa:    Nombres snake_case, enums como texto, decimales numeric(14,2), EXCLUDE USING gist
 *               (chapel_id WITH =, period WITH &&) vía migración SQL; client_uuid único.
 * Devuelve:   —
 * Se conecta: ← AppDbContext.OnModelCreating · → migraciones · → db/schema.sql
 * Estado:     Pendiente
 */
