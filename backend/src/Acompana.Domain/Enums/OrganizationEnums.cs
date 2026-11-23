/*
 * Archivo:    backend/src/Acompana.Domain/Enums/OrganizationEnums.cs
 * Qué hace:   Enumeraciones: Zone (GAM, Fuera) · EmployeeRole (Advisor, BranchManager, Driver,
 *               CrematoriumOperator, Management) · Shift (Day, Night) · DemoRole (Advisor,
 *               BranchManager, CrematoriumOperator, Provider, Management, Driver).
 * Recibe:     —
 * Procesa:    Se guardan como texto en PostgreSQL (HasConversion<string>) para que pgAdmin y el grafo
 *               se lean claro.
 * Devuelve:   Valores que viajan en los DTO como texto; la interfaz los traduce al español.
 * Se conecta: ← entidades, DTO, configuraciones EF · → GET /api/meta/enums
 * Reglas:     RF-30
 * Estado:     Completo
 */

namespace Acompana.Domain.Enums;

public enum Zone { GAM, Fuera }

public enum EmployeeRole { Advisor, BranchManager, Driver, CrematoriumOperator, Management }

public enum Shift { Day, Night }

public enum DemoRole { Advisor, BranchManager, CrematoriumOperator, Provider, Management, Driver }
