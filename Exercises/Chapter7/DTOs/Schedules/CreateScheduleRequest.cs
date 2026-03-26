using System.ComponentModel.DataAnnotations;

namespace Chapter6.DTOs.Schedules;

public class CreateScheduleRequest
{
    [Required]
    public int CarId { get; set; }

    [Required]
    public int DriverId { get; set; }

    [Required]
    public int PickupDestinationId { get; set; }

    [Required]
    public int DropoffDestinationId { get; set; }

    public DateTime StartTime { get; set; }
    public DateTime EndTime { get; set; }
}
