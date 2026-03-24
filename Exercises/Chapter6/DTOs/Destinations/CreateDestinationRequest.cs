using System.ComponentModel.DataAnnotations;

namespace Chapter6.DTOs.Destinations;

public class CreateDestinationRequest
{
    [Required]
    [MaxLength(120)]
    public string Name { get; set; } = string.Empty;

    [MaxLength(250)]
    public string? Address { get; set; }
}