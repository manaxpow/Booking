namespace Chapter6.DTOs.Auth;

public class RegisterResponse
{
    public int Id { get; set; }
    public string FullName { get; set; } = string.Empty;
    public string Phone { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Cccd { get; set; } = string.Empty;
    public string Role { get; set; } = "USER";
    public DateTime CreateAt { get; set; }
}
