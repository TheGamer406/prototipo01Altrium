/*
 * Archivo:    backend/src/Acompana.Infrastructure/Persistence/Configurations/BillingConfiguration.cs
 * Qué hace:   Mapeo EF Core de las tablas: invoices, payments.
 * Recibe:     ModelBuilder (lo llama AppDbContext).
 * Procesa:    Nombres snake_case, enums como texto, decimales numeric(14,2), idempotency_key único.
 * Devuelve:   —
 * Se conecta: ← AppDbContext.OnModelCreating · → migraciones · → db/schema.sql
 * Estado:     Pendiente
 */
