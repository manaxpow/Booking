public class SePayWebhookDto
{
    public long id { get; set; }           // ID giao dịch của SePay
    public string content { get; set; } = string.Empty;    // Nội dung: "VE123"
    public decimal transferAmount { get; set; }
    public string transferType { get; set; } = string.Empty; // "in"
    public string gateway { get; set; } = string.Empty;    // Ví dụ: "MBBank"
}