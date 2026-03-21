public static class SePayHelper
{
    private const string BaseQrUrl = "https://qr.sepay.vn/img";

    public static string GenerateQrUrl(
        string bankAccount,
        string bankName,
        decimal amount,
        string orderCode,
        bool isCompact = true)
    {
        var encodedDes = Uri.EscapeDataString(orderCode.Trim());
        var template = isCompact ? "compact" : "qr_only";

        // Link mẫu: https://qr.sepay.vn/img?acc=STK&bank=NH&amount=TIEN&des=NOIDUNG&template=compact
        return $"{BaseQrUrl}?acc={bankAccount}&bank={bankName}&amount={amount:0}&des={encodedDes}&template={template}";
    }
}