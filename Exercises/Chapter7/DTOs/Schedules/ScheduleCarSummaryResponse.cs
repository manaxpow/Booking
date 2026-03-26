namespace Chapter6.DTOs.Schedules;

public class ScheduleCarSummaryResponse
{
    public int Id { get; set; }
    public string LicensePlate { get; set; } = string.Empty;
    public string Brand { get; set; } = string.Empty;
    public string Model { get; set; } = string.Empty;
    public int SeatCount { get; set; }
}
