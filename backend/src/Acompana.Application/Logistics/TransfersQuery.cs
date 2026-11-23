/*
 * Archivo:    backend/src/Acompana.Application/Logistics/TransfersQuery.cs
 * Qué hace:   Traslados de la sede con última posición para el mapa.
 * Recibe:     branch?.
 * Procesa:    Traslados no terminados + hitos; delayed si algún tramo supera el umbral.
 * Devuelve:   TransferDto[].
 * Se conecta: ← GET /api/transfers
 * Reglas:     RF-23 · RF-24
 * Estado:     Pendiente
 */
