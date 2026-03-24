namespace Chapter6.DTOs.Schedules;

public class ScheduleSummaryResponse
{
    public int Id { get; set; }
    public int CarId { get; set; }
    public int DriverId { get; set; }
    public string DriverName { get; set; } = string.Empty;
    public int PickupDestinationId { get; set; }
    public string PickupDestinationName { get; set; } = string.Empty;
    public int DropoffDestinationId { get; set; }
    public string DropoffDestinationName { get; set; } = string.Empty;
    public DateTime StartTime { get; set; }
    public DateTime EndTime { get; set; }
    public bool IsActive { get; set; }
    public string Status { get; set; } = string.Empty;
    public int TotalSeatBookings { get; set; }
    public int TotalBookedSeats { get; set; }
    public ScheduleCarSummaryResponse? Car { get; set; }
}