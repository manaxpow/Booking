public record PaymentInfoResponse
{
    public int TicketId { get; init; }
    public decimal Amount { get; init; }
    public string OrderCode { get; init; } = null!;

    public string BankName { get; init; } = null!;
    public string AccountNumber { get; init; } = null!;
    public string AccountHolder { get; init; } = null!;

    public string QrUrl { get; init; } = null!;

    public DateTime ExpiresAt { get; init; }
    public PaymentStatus Status { get; init; } = PaymentStatus.PENDING;
}