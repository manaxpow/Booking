public class UserQueryParameters
{
    public string? Keyword { get; set; } // Search in name/phone/email/cccd
    public string? Role { get; set; }
    public string? Phone { get; set; }
    public string? Email { get; set; }
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 10;
}
