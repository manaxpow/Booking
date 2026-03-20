public class TicketDetail
{
    public int Id { get; set; }
    public int TicketId { get; set; }
    public int SeatBookingId { get; set; }

    public virtual Ticket Ticket { get; set; } = null!;
    public virtual SeatBooking SeatBooking { get; set; } = null!;
}