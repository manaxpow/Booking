public class SeatBooking
{
    public int Id { get; set; }
    public int ScheduleId { get; set; }
    public int SeatId { get; set; }
    public bool IsBooking { get; set; } = false;
    public bool IsHold { get; set; } = false;
    public DateTime CreateAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdateAt { get; set; } = DateTime.UtcNow;

    public virtual Schedule Schedule { get; set; } = new Schedule();
    public virtual Seat Seat { get; set; } = new Seat();
    public virtual Ticket Ticket { get; set; } = new Ticket();
}