public record BookingResponse
{
    public int TicketId { get; init; }
    public List<string> SeatNames { get; init; } = null!;

    public decimal TotalAmount { get; init; }
    public string OrderCode { get; init; } = null!;
    public string QrUrl { get; init; } = null!;
    public string Status { get; init; } = "PENDING";
    public DateTime CreatedAt { get; init; } = DateTime.UtcNow;
}