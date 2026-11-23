/*
 * Archivo:    backend/src/Acompana.Application/Admin/SimulationHandler.cs
 * Qué hace:   Simulaciones del modo admin para el demo.
 * Recibe:     tipo de simulación y parámetros.
 * Procesa:    ConcurrentAccept: elige una línea Offered con ≥ 2 ofertas y lanza dos AcceptOfferHandler
 *               en paralelo (Task.WhenAll)
 *             NextInvoiceReject: IMockControlClient.RejectNextInvoiceAsync
 *             CoreDown(minutes): IMockControlClient.SetCoreDownAsync
 * Devuelve:   { description, results[] } para mostrar en un snackbar.
 * Se conecta: ← POST /api/admin/simulate/* · → AcceptOfferHandler, IMockControlClient
 * Reglas:     RF-16 · RF-13
 * Estado:     Pendiente
 */
