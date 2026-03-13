public interface IAuthService
{
    Task<string> LoginAsync(string username, string password);
    Task<string> RegisterAsync(string phoneNumber, string password, string fullName, string email, string cccd);

}