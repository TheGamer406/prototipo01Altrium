/*
 * Archivo:    backend/src/Acompana.Domain/Oversight/Alert.cs
 * Qué hace:   Entidad Alert, se guarda en la tabla `alerts`.
 * Recibe:     Campos: Type (AlertType), BranchId, ServiceId?, Message, CreatedAt, AcknowledgedAt?. Los
 *               llena el handler que la crea o EF Core al leer.
 * Procesa:    Acknowledge(now).
 * Devuelve:   Nada; es estado. Hereda Id, CreatedAt y UpdatedAt de Entity.
 * Se conecta: ← jobs y handlers que detectan el problema · → campana de alertas, hub (AlertRaised)
 * Reglas:     RF-17 · RF-24 · RF-25 · RF-27
 * Estado:     Pendiente
 */
