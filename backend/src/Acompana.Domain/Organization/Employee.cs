/*
 * Archivo:    backend/src/Acompana.Domain/Organization/Employee.cs
 * Qué hace:   Entidad Employee, se guarda en la tabla `employees`.
 * Recibe:     Campos: BranchId, FullName, Role (EmployeeRole), Shift. Los llena el handler que la crea
 *               o EF Core al leer.
 * Procesa:    Sin lógica. En sedes fuera de la GAM una persona cumple Advisor + BranchManager.
 * Devuelve:   Nada; es estado. Hereda Id, CreatedAt y UpdatedAt de Entity.
 * Se conecta: ← StaffAssignment, Transfer (DriverId), Cremation (OperatorId)
 * Reglas:     RF-25 · RNF-09
 * Estado:     Pendiente
 */
