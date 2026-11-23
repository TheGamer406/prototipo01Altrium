/*
 * Archivo:    backend/src/Acompana.Api/Middleware/DemoCurrentUser.cs
 * Qué hace:   Implementación scoped de ICurrentUser llenada por DemoUserMiddleware.
 * Recibe:     Rol, sede, providerId (del middleware).
 * Procesa:    Solo guarda valores.
 * Devuelve:   Propiedades de ICurrentUser.
 * Se conecta: ← DemoUserMiddleware · → handlers, AuditInterceptor
 * Reglas:     RF-30
 * Estado:     Pendiente
 */
