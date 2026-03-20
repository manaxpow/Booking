public class SeatBooking
{
    public int Id { get; set; }
    public int ScheduleId { get; set; }
    public int SeatId { get; set; }
    public bool IsHold { get; set; } = false;
    public bool IsBooking { get; set; } = false;
    public DateTime CreateAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdateAt { get; set; } = DateTime.UtcNow;

    public virtual Schedule Schedule { get; set; } = null!;
    public virtual Seat Seat { get; set; } = null!;
    public virtual Ticket Ticket { get; set; } = null!;
}