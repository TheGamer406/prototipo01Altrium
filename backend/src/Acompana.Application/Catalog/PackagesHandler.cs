/*
 * Archivo:    backend/src/Acompana.Application/Catalog/PackagesHandler.cs
 * Qué hace:   Listar, crear y editar paquetes sin programar (gerencia).
 * Recibe:     filtros (active?, category?, branch?) para listar; DTO para crear/editar.
 * Procesa:    CRUD simple; precio = suma de componentes; editar un paquete no cambia servicios ya
 *               registrados; solo rol Management escribe.
 * Devuelve:   Lista de DTO o el DTO guardado.
 * Se conecta: ← CatalogController (GET/POST/PUT) · → AppDbContext
 * Reglas:     RF-04 · RF-06
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
