public class Payment
{
    public int Id { get; set; }
    public int TicketId { get; set; }
    public decimal Amount { get; set; }
    public string OrderCode { get; set; } = null!;
    public string? TransactionId { get; set; }
    public PaymentStatus Status { get; set; } = PaymentStatus.PENDING;
    public DateTime? ExpiredAt { get; set; } = DateTime.UtcNow.AddMinutes(10);
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public virtual Ticket Ticket { get; set; } = null!;
}