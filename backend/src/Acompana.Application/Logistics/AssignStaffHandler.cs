/*
 * Archivo:    backend/src/Acompana.Application/Logistics/AssignStaffHandler.cs
 * Qué hace:   Asigna personal del turno a un servicio y avisa traslapes (no bloquea).
 * Recibe:     AssignStaffRequest.
 * Procesa:    Exige servicio Paid o InProgress (RF-08); inserta; busca otra asignación del mismo
 *               empleado que se traslape; si hay, Alert StaffOverlap.
 * Devuelve:   AssignStaffResult { id, overlapWith? }.
 * Se conecta: ← POST /api/staff-assignments
 * Reglas:     RF-08 · RF-25
 * Estado:     Pendiente
 */
