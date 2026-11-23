/*
 * Archivo:    backend/src/Acompana.Api/Middleware/DemoUserMiddleware.cs
 * Qué hace:   Convierte X-Demo-Role y X-Demo-Branch en el usuario actual del request.
 * Recibe:     Encabezados HTTP (o query ?role=&branch= para SignalR).
 * Procesa:    Parsea rol (default Advisor) y sede (default San José); Management sin sede = todas;
 *               guarda en DemoCurrentUser (scoped).
 * Devuelve:   Continúa el pipeline; 400 si el rol no existe.
 * Se conecta: → DemoCurrentUser (ICurrentUser) · ← Program.cs (UseMiddleware)
 * Reglas:     RF-30
 * Estado:     Pendiente
 */
