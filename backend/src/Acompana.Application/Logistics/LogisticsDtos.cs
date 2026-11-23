/*
 * Archivo:    backend/src/Acompana.Application/Logistics/LogisticsDtos.cs
 * Qué hace:   DTO de capillas, personal, traslados y cremación.
 * Recibe:     —
 * Procesa:    —
 * Devuelve:   ChapelCalendarDto { chapels[{id, name, capacity, bookings[{serviceCode, from, to}]}] }
 *             BookChapelRequest { chapelId, serviceId, from, to }
 *             AssignStaffRequest { serviceId, employeeId, from, to } → AssignStaffResult { id,
 *               overlapWith?: serviceCode }
 *             TransferDto { id, serviceCode, plate, driverName, status, lastLat?, lastLng?,
 *               milestones[], delayed }
 *             MilestoneBatch { items[{clientUuid, type, recordedAt, lat?, lng?, source}] } → {
 *               accepted, duplicates }
 *             CremationCardDto { id, serviceCode, deceasedName, status, queuePosition, times{...},
 *               operator? }
 * Se conecta: ← LogisticsController · → core/models/logistics.models.ts
 * Reglas:     RF-20 a RF-25
 * Estado:     Completo
 */

namespace Acompana.Application.Logistics;

public record ChapelBookingDto(Guid Id, string ServiceCode, DateTimeOffset From, DateTimeOffset To);
public record ChapelDayDto(long Id, string Name, int Capacity, IReadOnlyList<ChapelBookingDto> Bookings);
public record ChapelCalendarDto(IReadOnlyList<ChapelDayDto> Chapels);
public record BookChapelRequest(long ChapelId, Guid ServiceId, DateTimeOffset From, DateTimeOffset To);

public record CremationCardDto(long Id, string ServiceCode, string DeceasedName, string Status, int QueuePosition, string? Operator,
    DateTimeOffset WaitingAt, DateTimeOffset? InOvenAt, DateTimeOffset? PackedAt, DateTimeOffset? DeliveredAt, string? ReceivedBy);
public record CremationBoardDto(IReadOnlyList<CremationCardDto> Waiting, IReadOnlyList<CremationCardDto> InOven, IReadOnlyList<CremationCardDto> Packed, IReadOnlyList<CremationCardDto> Delivered);
public record AdvanceCremationRequest(long OperatorId, string? ReceivedBy);
public record AssignStaffRequest(Guid ServiceId, long EmployeeId, DateTimeOffset From, DateTimeOffset To);
public record AssignStaffResult(Guid Id, string? OverlapWith);
public record EmployeeDto(long Id, string Name, string Role, string Shift, long BranchId);
public record StaffAssignmentDto(Guid Id, string ServiceCode, long EmployeeId, string EmployeeName, DateTimeOffset From, DateTimeOffset To);
public record StaffBoardDto(IReadOnlyList<EmployeeDto> Employees, IReadOnlyList<StaffAssignmentDto> Assignments);

public record MilestoneDto(Guid ClientUuid, string Type, DateTimeOffset RecordedAt, DateTimeOffset SyncedAt, double? Lat, double? Lng, string Source);
public record TransferDto(long Id, string ServiceCode, string Plate, string DriverName, string Status, double? LastLat, double? LastLng,
    IReadOnlyList<MilestoneDto> Milestones, bool Delayed);
public record MilestoneInput(Guid ClientUuid, string Type, DateTimeOffset RecordedAt, double? Lat, double? Lng, string Source);
public record MilestoneBatch(List<MilestoneInput> Items);
public record MilestoneResult(int Accepted, int Duplicates);
