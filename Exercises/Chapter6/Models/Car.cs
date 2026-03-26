using System.ComponentModel.DataAnnotations;

namespace Chapter6.Models;

public class Car
{
	public int Id { get; set; }

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

	public bool IsActive { get; set; } = true;

	public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

	public ICollection<Schedule> Schedules { get; set; } = new List<Schedule>();
}
