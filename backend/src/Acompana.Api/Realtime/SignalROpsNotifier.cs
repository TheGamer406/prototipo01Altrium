/*
 * Archivo:    backend/src/Acompana.Api/Realtime/SignalROpsNotifier.cs
 * Qué hace:   Implementa IOpsNotifier enviando eventos al hub.
 * Recibe:     Llamadas de handlers y jobs con el payload del evento.
 * Procesa:    IHubContext<OpsHub>.Clients.Groups(sede, 'management').SendAsync(evento, payload).
 * Devuelve:   Task.
 * Se conecta: ← Application (IOpsNotifier) · → OpsHub
 * Reglas:     RNF-06
 * Estado:     Pendiente
 */
