using System.ComponentModel.DataAnnotations;

namespace Chapter6.DTOs.Auth;

public class RefreshTokenRequest
{
    [Required(ErrorMessage = "RefreshToken là bắt buộc.")]
    public string RefreshToken { get; set; } = string.Empty;
}
