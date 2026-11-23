/*
 * Archivo:    backend/src/Acompana.Domain/Common/Entity.cs
 * Qué hace:   Clase base de todas las entidades: Id, CreatedAt, UpdatedAt.
 * Recibe:     Nada; las fechas las pone AppDbContext al guardar (UTC, se muestran en
 *               America/Costa_Rica).
 * Procesa:    Solo define propiedades comunes. Las entidades editables exponen además `Version` (xmin)
 *               como token de concurrencia.
 * Devuelve:   —
 * Se conecta: ← todas las entidades · → AppDbContext (marca fechas en SaveChanges)
 * Reglas:     RNF-13
 * Estado:     Pendiente
 */
