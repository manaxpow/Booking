using System.ComponentModel.DataAnnotations;

namespace Chapter6.Models;

public class Schedule
{
	public int Id { get; set; }

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

	public bool IsActive { get; set; } = true;

	[MaxLength(50)]
	public string Status { get; set; } = "INACTIVE";

	public Car? Car { get; set; }
	public Driver? Driver { get; set; }
	public Destination? PickupDestination { get; set; }
	public Destination? DropoffDestination { get; set; }
	public ICollection<SeatBooking> SeatBookings { get; set; } = new List<SeatBooking>();
}
