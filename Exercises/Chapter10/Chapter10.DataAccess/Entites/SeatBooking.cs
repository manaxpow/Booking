namespace Chapter10.DataAccess.Entities;

public class SeatBooking
{
    public int Id { get; set; }
    public int ScheduleId { get; set; }
    public string SeatNumber { get; set; } = string.Empty;
    public bool IsBooking { get; set; }
    public bool IsHold { get; set; }
}