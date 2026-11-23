/*
 * Archivo:    backend/src/Acompana.Api/Workers/OpsMonitorWorker.cs
 * Qué hace:   Trabajo en segundo plano cada 30 s.
 * Recibe:     Nada; se ejecuta solo (BackgroundService con PeriodicTimer).
 * Procesa:    Crea un scope y llama OfferTimeoutChecker y TransferDelayChecker.
 * Devuelve:   Nada; las alertas salen por IOpsNotifier.
 * Se conecta: → Application/Providers/OfferTimeoutChecker, Application/Logistics/TransferDelayChecker
 * Reglas:     RF-17 · RF-24
 * Estado:     Pendiente
 */
