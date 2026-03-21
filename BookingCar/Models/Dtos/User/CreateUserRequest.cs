public class CreateUserRequest
{
    public required string Name { get; set; }
    public required string Phone { get; set; }
    public required string Email { get; set; }
    public required string Password { get; set; }
    public required string Cccd { get; set; }
    public string Role { get; set; } = "USER";
}