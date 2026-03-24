using System.ComponentModel.DataAnnotations;

namespace Chapter6.Models;

public class Destination
{
    public int Id { get; set; }

    [Required]
    [MaxLength(120)]
    public string Name { get; set; } = string.Empty;

    [MaxLength(250)]
    public string? Address { get; set; }

    public bool IsActive { get; set; } = true;

    public ICollection<Schedule> PickupSchedules { get; set; } = new List<Schedule>();
    public ICollection<Schedule> DropoffSchedules { get; set; } = new List<Schedule>();
}