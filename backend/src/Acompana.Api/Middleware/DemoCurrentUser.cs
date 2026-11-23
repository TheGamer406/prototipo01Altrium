/*
 * Archivo:    backend/src/Acompana.Api/Middleware/DemoCurrentUser.cs
 * Qué hace:   Implementación scoped de ICurrentUser llenada por DemoUserMiddleware.
 * Recibe:     Rol, sede, providerId (del middleware).
 * Procesa:    Solo guarda valores.
 * Devuelve:   Propiedades de ICurrentUser.
 * Se conecta: ← DemoUserMiddleware · → handlers, AuditInterceptor
 * Reglas:     RF-30
 * Estado:     Completo
 */

using Acompana.Application.Common;
using Acompana.Domain.Enums;

namespace Acompana.Api.Middleware;

public sealed class DemoCurrentUser : ICurrentUser
{
    public DemoRole Role { get; set; } = DemoRole.Management;
    public long? BranchId { get; set; }
    public string DisplayName { get; set; } = "Sistema";
    public bool SeesAllBranches => Role == DemoRole.Management;
    public long? ProviderId { get; set; }
}
