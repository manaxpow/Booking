public record SeatBookingRequest
{
    public List<int> SeatBookingIds { get; init; } = new();
    public int UserId { get; set; }
}