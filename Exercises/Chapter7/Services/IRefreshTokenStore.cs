namespace Chapter6.Services;

public interface IRefreshTokenStore
{
    (string token, DateTime expiresAt) Generate(int userId);

    bool TryConsume(string token, out int userId);
}
