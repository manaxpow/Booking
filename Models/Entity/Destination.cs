public class Destination
{
    public int Id { get; set; }
    public string Province { get; set; } = string.Empty;
    public DateTime CreateAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdateAt { get; set; } = DateTime.UtcNow;

    public virtual ICollection<Schedule> FromSchedules { get; set; } = new List<Schedule>();
    public virtual ICollection<Schedule> ToSchedules { get; set; } = new List<Schedule>();
}