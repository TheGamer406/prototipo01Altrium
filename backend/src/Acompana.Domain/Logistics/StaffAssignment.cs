/*
 * Archivo:    backend/src/Acompana.Domain/Logistics/StaffAssignment.cs
 * Qué hace:   Entidad StaffAssignment, se guarda en la tabla `staff_assignments`.
 * Recibe:     Campos: ServiceId, EmployeeId, Period (tstzrange). Los llena el handler que la crea o EF
 *               Core al leer.
 * Procesa:    El traslape se permite; lo detecta AssignStaffHandler y genera alerta.
 * Devuelve:   Nada; es estado. Hereda Id, CreatedAt y UpdatedAt de Entity.
 * Se conecta: ← AssignStaffHandler
 * Reglas:     RF-25
 * Estado:     Pendiente
 */
