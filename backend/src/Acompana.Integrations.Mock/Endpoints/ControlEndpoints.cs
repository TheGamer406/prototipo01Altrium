/*
 * Archivo:    backend/src/Acompana.Integrations.Mock/Endpoints/ControlEndpoints.cs
 * Qué hace:   Control del simulador desde el modo admin.
 * Recibe:     POST /_control/core-down?minutes= · POST /_control/reject-next.
 * Procesa:    Fija CoreDownUntil = now + minutos · RejectNext = true (se consume en la próxima
 *               factura).
 * Devuelve:   200.
 * Se conecta: ← MockControlClient
 * Estado:     Pendiente
 */
