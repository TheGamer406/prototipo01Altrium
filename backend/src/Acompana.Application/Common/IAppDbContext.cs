/*
 * Archivo:    backend/src/Acompana.Application/Common/IAppDbContext.cs
 * Qué hace:   Interfaz del contexto de BD que usan los handlers (lo implementa Infrastructure).
 * Recibe:     —
 * Procesa:    DbSet<> de todas las entidades + SaveChangesAsync + BeginTransactionAsync +
 *               ExecuteSqlAsync (para la adjudicación y el refresco de vistas).
 * Devuelve:   —
 * Se conecta: ← handlers · → Infrastructure/Persistence/AppDbContext
 * Estado:     Pendiente
 */
