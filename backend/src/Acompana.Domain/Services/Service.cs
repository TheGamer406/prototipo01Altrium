/*
 * Archivo:    backend/src/Acompana.Domain/Services/Service.cs
 * Qué hace:   Agregado principal: un servicio funerario (tabla `services`) con sus líneas, acta e
 *               historial.
 * Recibe:     Campos: Code (SRV-2026-00001), BranchId, Kind, Status, DeceasedId, FamilyContactId,
 *               BasePackageId, IsCustomPackage, WakePlace, WakeAddress?, FinalDestination,
 *               CemeteryName?, ScheduledAt, Payer, InsurerId?, PolicyNumber?, TotalCrc, Version (xmin).
 * Procesa:    ChangeStatus(to, actor): valida la transición permitida
 *               (Advance→Registered→Invoicing→Paid→InProgress→Completed; cualquiera→Cancelled) y agrega
 *               un StatusHistory.
 *             RecalculateTotal(): suma LineTotalCrc de las líneas.
 *             MarkCustom(): IsCustomPackage = true cuando cambian las líneas respecto al paquete base.
 * Devuelve:   Nada; lanza DomainException si la transición no es válida.
 * Se conecta: ← handlers de Services, Billing, Providers, Logistics · → ServiceLine, DeathCertificate,
 *               StatusHistory
 * Reglas:     RF-01 · RF-02 · RF-05 · RF-07 · RF-08 · RF-11
 * Estado:     Pendiente
 *
 * Contrato:
 *   void ChangeStatus(ServiceStatus to, string actor, DateTimeOffset now)
 *     Recibe:     estado destino, nombre del actor (del usuario demo), hora actual
 *     Procesa:    busca (Status, to) en la tabla de transiciones; si es Invoicing exige
 *                   DeathCertificate cuando Kind = Immediate (RF-03); si es InProgress exige Status =
 *                   Paid (RF-08)
 *     Devuelve:   void; agrega StatusHistory(from, to, actor, now)
 *     Errores:    DomainException 'Registre el acta de defunción antes de facturar.' · 'El servicio no
 *                   puede iniciar sin factura pagada.' · 'Transición no permitida.'
 *   decimal RecalculateTotal()
 *     Recibe:     nada (usa Lines)
 *     Procesa:    TotalCrc = Σ line.LineTotalCrc
 *     Devuelve:   el nuevo TotalCrc
 */
