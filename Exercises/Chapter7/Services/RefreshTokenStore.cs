using System.Collections.Concurrent;
using System.Security.Cryptography;
using Chapter6.Services.Common;

namespace Chapter6.Services;

public class RefreshTokenStore : IRefreshTokenStore
{
    private static readonly TimeSpan TokenLifetime = TimeSpan.FromDays(30);
    private readonly ConcurrentDictionary<string, RefreshTokenEntry> _store = new();

    public (string token, DateTime expiresAt) Generate(int userId)
    {
        var token = RandomNumberGenerator.GetHexString(64, lowercase: false);
        var expiresAt = DateTime.UtcNow.Add(TokenLifetime);

        _store[token] = new RefreshTokenEntry
        {
            UserId = userId,
            ExpiresAt = expiresAt,
            IsRevoked = false,
        };

        return (token, expiresAt);
    }

    public bool TryConsume(string token, out int userId)
    {
        userId = 0;

        if (!_store.TryGetValue(token, out var entry))
            return false;

        if (entry.IsRevoked || DateTime.UtcNow > entry.ExpiresAt)
            return false;

        // Token rotation: revoke ngay sau khi consume
        entry.IsRevoked = true;
        userId = entry.UserId;
        return true;
    }
}
