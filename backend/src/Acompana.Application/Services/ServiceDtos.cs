/*
 * Archivo:    backend/src/Acompana.Application/Services/ServiceDtos.cs
 * Qué hace:   DTO de entrada y salida del módulo Servicios.
 * Recibe:     —
 * Procesa:    Records inmutables. Montos en decimal (colones), fechas en DateTimeOffset.
 * Devuelve:   CreateServiceRequest { kind, branchId, deceased{fullName, nationalId, birthDate,
 *               deathDate?}, family{fullName, nationalId, relationship, phone, email,
 *               insuranceProspectConsent}, basePackageId, lines[{componentId, casketModelId?, qty}],
 *               wakePlace, wakeAddress?, chapelId?, finalDestination, cemeteryName?, scheduledAt, payer,
 *               insurerId?, policyNumber? }
 *             ServiceListItemDto { id, code, deceasedName, branchName, status, kind, scheduledAt,
 *               totalCrc, isCustomPackage, hasUrgentLine }
 *             ServiceDetailDto { ...ListItem, deceased, family, basePackage, lines[],
 *               deathCertificate?, invoice?, wake, destination, payer }
 *             ServiceLineDto { id, componentName, category, imagePath?, qty, unitPriceCrc,
 *               taxExemptPct, lineTotalCrc, status, providerName?, acceptedPriceCrc?, acceptedAt?,
 *               dueAt?, isUrgent }
 *             ServiceBoardDto { serviceId, code, lines: ServiceLineDto[] }
 *             TimelineEntryDto { at, kind ('status'|'audit'|'message'), text }
 * Se conecta: ← ServicesController · → features/services (core/models/services.models.ts debe
 *               coincidir)
 * Reglas:     RF-01 a RF-07 · RF-19
 * Estado:     Pendiente
 */
