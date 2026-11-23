/*
 * Archivo:    backend/src/Acompana.Application/Common/ICurrentUser.cs
 * Qué hace:   Contrato del usuario demo actual (no hay login).
 * Recibe:     Lo implementa Api/Middleware/DemoCurrentUser a partir de X-Demo-Role y X-Demo-Branch.
 * Procesa:    Expone Role (DemoRole), BranchId (long?), DisplayName (p. ej. 'Asesor · Heredia'),
 *               SeesAllBranches (= Role == Management), ProviderId? (solo rol Proveedor).
 * Devuelve:   —
 * Se conecta: ← todos los handlers (filtro por sede) · → AuditInterceptor (actor)
 * Reglas:     RF-30
 * Estado:     Pendiente
 */
