public class Ticket
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public TicketStatus Status { get; set; } = TicketStatus.PENDING;
    public decimal TotalPrice { get; set; }
    public DateTime CreateAt
    { get; set; } = DateTime.UtcNow;
    public DateTime UpdateAt { get; set; } = DateTime.UtcNow;

    public virtual User User { get; set; } = null!;
    public virtual SeatBooking SeatBooking { get; set; } = null!;
}