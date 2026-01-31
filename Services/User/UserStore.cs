public class UserStore : IUserStore
{
    public List<User> Users { get; } = new()
    {
        new User { FullName = "Admin User", Username = "admin", PasswordHash = "admin123" },
        new User { FullName = "Regular User", Username = "user", PasswordHash = "user123" },
        new User { FullName = "Guest User", Username = "guest", PasswordHash = "guest123" }
    };
}