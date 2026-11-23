/*
 * Archivo:    backend/src/Acompana.Domain/Common/Entity.cs
 * Qué hace:   Clase base de todas las entidades: Id, CreatedAt, UpdatedAt.
 * Recibe:     Nada; las fechas las pone AppDbContext al guardar (UTC, se muestran en
 *               America/Costa_Rica).
 * Procesa:    EntityBase (fechas), Entity (Id long), GuidEntity (Id Guid) e IHasBranch. Solo define propiedades comunes. Las entidades editables exponen además `Version` (xmin)
 *               como token de concurrencia.
 * Devuelve:   —
 * Se conecta: ← todas las entidades · → AppDbContext (marca fechas en SaveChanges)
 * Reglas:     RNF-13
 * Estado:     Completo
 */

namespace Acompana.Domain.Common;

/// <summary>Marcas de tiempo comunes (UTC); las pone AppDbContext al guardar.</summary>
public abstract class EntityBase
{
    public DateTimeOffset CreatedAt { get; set; }
    public DateTimeOffset UpdatedAt { get; set; }
}

/// <summary>Entidad con identificador numérico (identity).</summary>
public abstract class Entity : EntityBase
{
    public long Id { get; set; }
}

/// <summary>Entidad con identificador Guid (se genera en la aplicación).</summary>
public abstract class GuidEntity : EntityBase
{
    public Guid Id { get; set; } = Guid.NewGuid();
}

/// <summary>Entidad que pertenece a una sede (filtro BranchScope).</summary>
public interface IHasBranch
{
    long BranchId { get; }
}
