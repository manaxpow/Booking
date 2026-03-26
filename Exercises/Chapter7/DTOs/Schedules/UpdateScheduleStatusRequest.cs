using System.ComponentModel.DataAnnotations;

namespace Chapter6.DTOs.Schedules;

public class UpdateScheduleStatusRequest
{
    [Required]
    [MaxLength(20)]
    public string Status { get; set; } = string.Empty;
}
