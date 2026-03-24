namespace Chapter6.Models;

public class SeatBooking
{
    public int Id { get; set; }
    public int ScheduleId { get; set; }
    public int? UserId { get; set; }
    public int SeatNumber { get; set; }
    public bool IsBooking { get; set; }
    public bool IsHold { get; set; }

    public Schedule? Schedule { get; set; }
    public User? User { get; set; }
}
