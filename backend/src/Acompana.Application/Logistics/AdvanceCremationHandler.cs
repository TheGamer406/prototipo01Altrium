/*
 * Archivo:    backend/src/Acompana.Application/Logistics/AdvanceCremationHandler.cs
 * Qué hace:   Mueve una cremación al siguiente estado.
 * Recibe:     cremationId + { operatorId, receivedBy? }.
 * Procesa:    Cremation.Advance(...); al entregar, FamilyNotifier (cenizas entregadas); notifica
 *               CremationMoved.
 * Devuelve:   CremationCardDto actualizado.
 * Se conecta: ← POST /api/cremations/{id}/advance
 * Reglas:     RF-21 · RF-35
 * Estado:     Pendiente
 */
