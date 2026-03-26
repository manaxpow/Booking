using System.ComponentModel.DataAnnotations;

namespace Chapter6.DTOs.Auth;

public class RegisterRequest
{
    [Required]
    [MaxLength(150)]
    public string FullName { get; set; } = string.Empty;

    [Required]
    [MaxLength(20)]
    public string Phone { get; set; } = string.Empty;

    [Required]
    [EmailAddress]
    [MaxLength(150)]
    public string Email { get; set; } = string.Empty;

    [Required]
    [MinLength(6)]
    [MaxLength(100)]
    public string Password { get; set; } = string.Empty;

    [Required]
    [MaxLength(20)]
    public string Cccd { get; set; } = string.Empty;

    [MaxLength(20)]
    public string Role { get; set; } = "USER";
}
