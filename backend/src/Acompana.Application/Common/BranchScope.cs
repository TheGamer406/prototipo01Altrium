/*
 * Archivo:    backend/src/Acompana.Application/Common/BranchScope.cs
 * Qué hace:   Filtro por sede reutilizable para IQueryable.
 * Recibe:     IQueryable<T> con BranchId e ICurrentUser; branch opcional pedido por query string.
 * Procesa:    Si SeesAllBranches: aplica el branch pedido o nada. Si no: fuerza BranchId =
 *               usuario.BranchId (ignora el pedido).
 * Devuelve:   IQueryable<T> filtrado.
 * Se conecta: ← todas las queries de listas
 * Reglas:     RF-30
 * Estado:     Completo
 *
 * Contrato:
 *   static IQueryable<T> ForUser<T>(this IQueryable<T> q, ICurrentUser user, long? branch) where T :
 *       IHasBranch
 *     Recibe:     query, usuario, sede pedida
 *     Procesa:    ver arriba
 *     Devuelve:   query filtrada
 */

using Acompana.Domain.Common;

namespace Acompana.Application.Common;

public static class BranchScope
{
    public static IQueryable<T> ForUser<T>(this IQueryable<T> q, ICurrentUser user, long? branch) where T : IHasBranch
    {
        if (user.SeesAllBranches)
            return branch is null ? q : q.Where(x => x.BranchId == branch.Value);
        var own = user.BranchId ?? -1;
        return q.Where(x => x.BranchId == own);
    }
}
