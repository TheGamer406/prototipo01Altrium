/*
 * Archivo:    backend/src/Acompana.Application/Providers/OfferTimeoutChecker.cs
 * Qué hace:   Detecta líneas sin aceptar dentro del plazo configurable.
 * Recibe:     now + Setting ProviderAcceptTimeoutMin (30 por defecto).
 * Procesa:    Líneas Offered con primera oferta enviada hace más del plazo y sin alerta previa:
 *               ofertas → Expired, crea Alert ProviderTimeout para la sede.
 * Devuelve:   int alertas creadas.
 * Se conecta: ← Api/Workers/OpsMonitorWorker (cada 30 s) · → IOpsNotifier (AlertRaised)
 * Reglas:     RF-17
 * Estado:     Pendiente
 */
