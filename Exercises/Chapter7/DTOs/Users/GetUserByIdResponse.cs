namespace Chapter6.DTOs.Users;

public class GetUserByIdResponse
{
    public int Id { get; set; }
    public string FullName { get; set; } = string.Empty;
    public string Phone { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Cccd { get; set; } = string.Empty;
    public string Role { get; set; } = string.Empty;
    public DateTime CreateAt { get; set; }
    public DateTime? UpdateAt { get; set; }
}
