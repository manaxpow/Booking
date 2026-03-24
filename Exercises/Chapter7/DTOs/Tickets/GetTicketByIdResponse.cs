namespace Chapter6.DTOs.Tickets;

public class GetTicketByIdResponse
{
    public int Id { get; set; }
    public int ScheduleId { get; set; }
    public int SeatNumber { get; set; }
    public bool IsBooking { get; set; }
    public bool IsHold { get; set; }
    public int? UserId { get; set; }
}
