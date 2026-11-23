/*
 * Archivo:    backend/src/Acompana.Application/Logistics/TransferDelayChecker.cs
 * Qué hace:   Alerta traslados con tramos más largos que el umbral.
 * Recibe:     now + Setting TransferThresholdMin (20).
 * Procesa:    Para cada traslado activo, tiempo entre hitos consecutivos (o desde el último hasta
 *               now); si supera el umbral y no hay alerta, crea Alert TransferDelay.
 * Devuelve:   int alertas creadas.
 * Se conecta: ← OpsMonitorWorker (cada 30 s)
 * Reglas:     RF-24
 * Estado:     Pendiente
 */
