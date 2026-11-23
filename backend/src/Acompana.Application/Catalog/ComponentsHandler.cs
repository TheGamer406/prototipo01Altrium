/*
 * Archivo:    backend/src/Acompana.Application/Catalog/ComponentsHandler.cs
 * Qué hace:   Listar, crear y editar componentes sin programar (gerencia).
 * Recibe:     filtros (active?, category?, branch?) para listar; DTO para crear/editar.
 * Procesa:    CRUD simple; valida TaxExemptPct ∈ {0,50,100}; solo rol Management escribe.
 * Devuelve:   Lista de DTO o el DTO guardado.
 * Se conecta: ← CatalogController (GET/POST/PUT) · → AppDbContext
 * Reglas:     RF-06 · RF-10
 * Estado:     Pendiente
 *
 * Contrato:
 *   Task<IReadOnlyList<TDto>> ListAsync(TFilter f, CancellationToken ct)
 *     Recibe:     filtros
 *     Procesa:    consulta
 *     Devuelve:   lista
 *   Task<TDto> SaveAsync(long? id, TDto dto, CancellationToken ct)
 *     Recibe:     id (null = nuevo) y datos
 *     Procesa:    valida y guarda
 *     Devuelve:   DTO guardado
 *     Errores:    403 si el rol no es Gerencia
 */
