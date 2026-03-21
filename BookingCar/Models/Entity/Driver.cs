public class Driver
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public DateTime Dob { get; set; }
    public string License { get; set; } = string.Empty;
    public DateTime CreateAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdateAt { get; set; } = DateTime.UtcNow;

    public virtual ICollection<Schedule> Schedules { get; set; } = new List<Schedule>();
}