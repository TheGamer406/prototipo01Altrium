/*
 * Archivo:    backend/src/Acompana.Infrastructure/Persistence/AuditInterceptor.cs
 * Qué hace:   Escribe audit_log por cada alta, cambio o baja.
 * Recibe:     SavingChanges de EF Core + ICurrentUser.
 * Procesa:    Recorre ChangeTracker (excepto AuditLog/OutboundMessage/StatusHistory); serializa a
 *               jsonb los valores originales y nuevos de las propiedades modificadas.
 * Devuelve:   Agrega filas AuditLog en el mismo SaveChanges.
 * Se conecta: ← AppDbContext · → AuditQuery (caso de las flores)
 * Reglas:     RF-31 · RNF-13
 * Estado:     Pendiente
 */
