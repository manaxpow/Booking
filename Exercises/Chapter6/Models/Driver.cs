using System.ComponentModel.DataAnnotations;

namespace Chapter6.Models;

public class Driver
{
    public int Id { get; set; }

    [Required]
    [MaxLength(100)]
    public string FullName { get; set; } = string.Empty;

    public bool IsActive { get; set; } = true;

    public ICollection<Schedule> Schedules { get; set; } = new List<Schedule>();
}