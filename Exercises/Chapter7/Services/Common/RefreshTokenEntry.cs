namespace Chapter6.Services.Common;

public class RefreshTokenEntry
{
    public int UserId { get; init; }
    public DateTime ExpiresAt { get; init; }
    public bool IsRevoked { get; set; }
}
