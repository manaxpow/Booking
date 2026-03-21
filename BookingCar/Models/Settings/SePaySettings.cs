public class SePaySettings
{
    public const string SectionName = "SePay";

    public string ApiKey { get; set; } = string.Empty;
    public string BankName { get; set; } = string.Empty;
    public string AccountNumber { get; set; } = string.Empty;
    public string AccountHolder { get; set; } = string.Empty;
}