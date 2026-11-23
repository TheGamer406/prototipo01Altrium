/*
 * Archivo:    backend/src/Acompana.Infrastructure/Persistence/AppDbContext.cs
 * Qué hace:   DbContext de EF Core (implementa IAppDbContext).
 * Recibe:     DbContextOptions (Npgsql) + AuditInterceptor.
 * Procesa:    DbSet de todas las entidades; ApplyConfigurationsFromAssembly; extensión btree_gist;
 *               esquema analytics; fija CreatedAt/UpdatedAt en SaveChanges; xmin como token de
 *               concurrencia.
 * Devuelve:   —
 * Se conecta: ← handlers (vía IAppDbContext) · → PostgreSQL acompana-db
 * Reglas:     RF-20 (btree_gist) · RF-31
 * Estado:     Pendiente
 */
