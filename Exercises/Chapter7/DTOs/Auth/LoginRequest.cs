using System.ComponentModel.DataAnnotations;

namespace Chapter6.DTOs.Auth;

public class LoginRequest
{
    [Required]
    [MaxLength(20)]
    public string Phone { get; set; } = string.Empty;

    [Required]
    [MaxLength(100)]
    public string Password { get; set; } = string.Empty;
}
