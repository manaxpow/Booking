public class Ticket
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public int SeatBookingId { get; set; } // Foreign key to SeatBooking
    public DateTime CreateAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdateAt { get; set; } = DateTime.UtcNow;

    public virtual User User { get; set; } = new User();
    public virtual SeatBooking SeatBooking { get; set; } = new SeatBooking();
}