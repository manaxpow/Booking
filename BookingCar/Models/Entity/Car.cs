public class Car
{
    public int Id { get; set; }
    public string LicensePlate { get; set; } = string.Empty;
    public int Capacity { get; set; }
    public string Brand { get; set; } = string.Empty;
    public DateTime CreateAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdateAt { get; set; } = DateTime.UtcNow;

    public virtual ICollection<Seat> Seats { get; set; } = new List<Seat>();
    public virtual ICollection<Schedule> Schedules { get; set; } = new List<Schedule>();
}