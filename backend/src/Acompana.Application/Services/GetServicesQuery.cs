/*
 * Archivo:    backend/src/Acompana.Application/Services/GetServicesQuery.cs
 * Qué hace:   Lista paginada de servicios con filtros.
 * Recibe:     status?, branch?, q? (código, nombre del fallecido o cédula), page, pageSize (25), sort.
 * Procesa:    BranchScope.ForUser; búsqueda ILIKE; orden por ScheduledAt desc por defecto; calcula
 *               hasUrgentLine.
 * Devuelve:   PagedResult<ServiceListItemDto>.
 * Se conecta: ← GET /api/services · también lo usa la búsqueda global de la barra superior
 * Reglas:     RF-19 · RF-30
 * Estado:     Pendiente
 */
