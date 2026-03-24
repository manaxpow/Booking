using System.ComponentModel.DataAnnotations;

namespace Chapter6.Models;

public class User
{
    public int Id { get; set; }

    [Required]
    [MaxLength(150)]
    public string FullName { get; set; } = string.Empty;

    [Required]
    [MaxLength(20)]
    public string Phone { get; set; } = string.Empty;

    [Required]
    [MaxLength(150)]
    [EmailAddress]
    public string Email { get; set; } = string.Empty;

    [Required]
    [MaxLength(300)]
    public string PasswordHash { get; set; } = string.Empty;

    [Required]
    [MaxLength(20)]
    public string Cccd { get; set; } = string.Empty;

    [Required]
    [MaxLength(20)]
    public string Role { get; set; } = "USER";

    public DateTime CreateAt { get; set; } = DateTime.UtcNow;

    public DateTime? UpdateAt { get; set; }

    public ICollection<SeatBooking> SeatBookings { get; set; } = new List<SeatBooking>();
}
