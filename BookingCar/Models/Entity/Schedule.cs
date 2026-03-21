public class Schedule
{
    public int Id { get; set; }
    public int CarId { get; set; }
    public int DriverId { get; set; }
    public int FromDestinationId { get; set; }
    public int ToDestinationId { get; set; }

    public DateTime StartTime { get; set; }
    public DateTime EndTime { get; set; }
    public TimeSpan ExpectedDuration { get; set; }

    public string Status { get; set; } = string.Empty; // ACTIVE, INACTIVE, FINISH 
    public decimal Price { get; set; }
    public DateTime CreateAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdateAt { get; set; } = DateTime.UtcNow;

    public virtual Car Car { get; set; } = null!;
    public virtual Driver Driver { get; set; } = null!;
    public virtual Destination FromDestination { get; set; } = null!;
    public virtual Destination ToDestination { get; set; } = null!;
    public virtual ICollection<SeatBooking> SeatBookings { get; set; } = new List<SeatBooking>();
}