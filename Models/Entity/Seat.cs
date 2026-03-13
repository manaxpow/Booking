public class Seat
{
    public int Id { get; set; }
    public int CarId { get; set; }
    public string Name { get; set; } = string.Empty;
    public DateTime CreateAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdateAt { get; set; } = DateTime.UtcNow;

    public virtual Car Car { get; set; } = new Car();
    public virtual ICollection<SeatBooking> SeatBookings { get; set; } = new List<SeatBooking>();
}