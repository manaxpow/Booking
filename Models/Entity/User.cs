public class User
{
    public required string FullName { get; set; }
    public required string Username { get; set; }
    public required string PasswordHash { get; set; }

    public User () { }
    public User (string fullName, string username, string passwordHash)
    {
        FullName = fullName;
        Username = username;
        PasswordHash = passwordHash;
    }
}