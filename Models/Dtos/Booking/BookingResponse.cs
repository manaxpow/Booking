public record BookingResponse
{
    public int TicketId { get; init; }
    public string TicketCode { get; init; } = null!;
    public string SeatName { get; init; } = null!;

    public decimal TotalAmount { get; init; }
    public string OrderCode { get; init; } = null!;
    public string QrUrl { get; init; } = null!;

    public string RouteName { get; init; } = null!;
    public DateTime StartTime { get; init; }

    public string Status { get; init; } = "PENDING";
    public DateTime CreatedAt { get; init; }
}