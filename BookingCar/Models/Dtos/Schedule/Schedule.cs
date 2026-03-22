using VehicleBooking.Models.DTOs.Common;

namespace Models.Dtos.Schedule;


public record CreateScheduleRequest(
    int CarId,
    int DriverId,
    int FromDestinationId,
    int ToDestinationId,
    DateTime StartTime,
    string Status,
    decimal Price,
    TimeSpan ExpectedDuration
);


public record UpdateScheduleRequest(
    int Id,
    int? CarId,
    int? DriverId,
    int? FromDestinationId,
    int? ToDestinationId,
    DateTime? StartTime,
    string? Status,
    decimal? Price
);


public record ScheduleQuery
{
    public int? CarId { get; init; }
    public int? DriverId { get; init; }
    public int? FromDestinationId { get; init; }
    public int? ToDestinationId { get; init; }
    public string? Status { get; init; }
    public DateTime? StartTimeFrom { get; init; }
    public DateTime? StartTimeTo { get; init; }
    public int Page { get; init; } = 1;
    public int PageSize { get; init; } = 10;
    public string? SortBy { get; init; }
    public bool SortDescending { get; init; } = false;
}


public record ScheduleResponse(
    int Id,
    int CarId,
    int DriverId,
    string? CarLicensePlate,
    string? DriverFullName,
    int? FromDestinationId,
    string? FromDestinationProvince,
    int? ToDestinationId,
    string? ToDestinationProvince,
    DateTime StartTime,
    DateTime EndTime,
    TimeSpan? ExpectedDuration,
    string? Status,
    decimal? Price,
    DateTime? CreateAt,
    DateTime? UpdateAt
)
{
    public static implicit operator ScheduleResponse(PagedResult<ScheduleResponse> v)
    {
        throw new NotImplementedException();
    }
}