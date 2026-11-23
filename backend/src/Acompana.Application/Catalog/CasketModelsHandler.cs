/*
 * Archivo:    backend/src/Acompana.Application/Catalog/CasketModelsHandler.cs
 * Qué hace:   Listar, crear y editar modelos de cofre sin programar (gerencia).
 * Recibe:     filtros (active?, category?, branch?) para listar; DTO para crear/editar.
 * Procesa:    CRUD simple; list acepta branch para incluir stock de la sede; solo rol Management
 *               escribe.
 * Devuelve:   Lista de DTO o el DTO guardado.
 * Se conecta: ← CatalogController (GET/POST/PUT) · → AppDbContext
 * Reglas:     RF-04 · RF-26
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
