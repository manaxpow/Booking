using System.ComponentModel.DataAnnotations;

namespace Chapter6.DTOs.Cars;

public class CreateCarRequest
{
    [Required]
    [MaxLength(20)]
    public string LicensePlate { get; set; } = string.Empty;

    [Required]
    [MaxLength(100)]
    public string Brand { get; set; } = string.Empty;

    [Required]
    [MaxLength(100)]
    public string Model { get; set; } = string.Empty;

    [Range(1, 100)]
    public int SeatCount { get; set; }

    [MaxLength(50)]
    public string? Color { get; set; }

    public int? ManufactureYear { get; set; }
}