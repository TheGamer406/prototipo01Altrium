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
 * Estado:     Completo
 */

using Acompana.Application.Billing;
using Acompana.Domain.Enums;

namespace Acompana.Application.Services;

public record DeceasedInput(string FullName, string NationalId, DateOnly BirthDate, DateOnly? DeathDate);
public record FamilyInput(string FullName, string NationalId, string Relationship, string Phone, string Email, bool InsuranceProspectConsent);
public record LineInput(long ComponentId, long? CasketModelId, int Qty);

public record CreateServiceRequest(
    ServiceKind Kind, long BranchId, DeceasedInput Deceased, FamilyInput Family, long BasePackageId,
    List<LineInput> Lines, WakePlace WakePlace, string? WakeAddress, long? ChapelId,
    FinalDestination FinalDestination, string? CemeteryName, DateTimeOffset ScheduledAt,
    Payer Payer, long? InsurerId, string? PolicyNumber);

public record UpdateServiceRequest(
    FamilyInput Family, WakePlace WakePlace, string? WakeAddress, FinalDestination FinalDestination,
    string? CemeteryName, DateTimeOffset ScheduledAt, Payer Payer, long? InsurerId, string? PolicyNumber, uint Version);

public record ActivateRequest(DateOnly DeathDate, DateTimeOffset ScheduledAt);
public record SetPackageRequest(long BasePackageId, List<LineInput> Lines);

public record ServiceListItemDto(
    Guid Id, string Code, string DeceasedName, string BranchName, string Status, string Kind,
    DateTimeOffset ScheduledAt, decimal TotalCrc, bool IsCustomPackage, bool HasUrgentLine);

public record DeceasedDto(string FullName, string NationalId, DateOnly BirthDate, DateOnly? DeathDate, string CauseNote);
public record FamilyDto(string FullName, string NationalId, string Relationship, string Phone, string Email, bool InsuranceProspectConsent);

public record ServiceLineDto(
    Guid Id, long ComponentId, string ComponentName, string Category, string? ImagePath, int Qty, decimal UnitPriceCrc,
    int TaxExemptPct, decimal LineTotalCrc, string Status, string? ProviderName, decimal? AcceptedPriceCrc,
    DateTimeOffset? AcceptedAt, DateTimeOffset? DueAt, bool IsUrgent, long? CasketModelId, string? CasketModelName);

public record DeathCertificateDto(string Number, string FileUrl, string RegisteredBy, DateTimeOffset RegisteredAt);
public record WakeDto(string Place, string? Address, long? ChapelId, string? ChapelName, DateTimeOffset? From, DateTimeOffset? To);
public record DestinationDto(string Kind, string? CemeteryName);
public record PayerDto(string Payer, long? InsurerId, string? InsurerName, string? PolicyNumber);

public record ServiceDetailDto(
    Guid Id, string Code, long BranchId, string BranchName, string Status, string Kind, DateTimeOffset ScheduledAt,
    decimal TotalCrc, bool IsCustomPackage, bool HasUrgentLine, DeceasedDto Deceased, FamilyDto Family,
    long? BasePackageId, string? BasePackage, IReadOnlyList<ServiceLineDto> Lines, DeathCertificateDto? DeathCertificate,
    InvoiceDto? Invoice, WakeDto Wake, DestinationDto Destination, PayerDto Payer, uint Version);

public record ServiceBoardDto(Guid ServiceId, string Code, IReadOnlyList<ServiceLineDto> Lines);
public record TimelineEntryDto(DateTimeOffset At, string Kind, string Text);
