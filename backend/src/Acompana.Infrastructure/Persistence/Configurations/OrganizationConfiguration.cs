/*
 * Archivo:    backend/src/Acompana.Infrastructure/Persistence/Configurations/OrganizationConfiguration
 *               .cs
 * Qué hace:   Mapeo EF Core de las tablas: branches, employees, deceased, family_contacts, insurers.
 * Recibe:     ModelBuilder (lo llama AppDbContext).
 * Procesa:    Nombres snake_case, enums como texto, decimales numeric(14,2), índice por national_id.
 * Devuelve:   —
 * Se conecta: ← AppDbContext.OnModelCreating · → migraciones · → db/schema.sql
 * Estado:     Pendiente
 */
