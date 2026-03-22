using System.Text.Json.Serialization;
using Microsoft.EntityFrameworkCore;

namespace Chapter9;

public enum SeatStatus { Available, Hold, Sold }

public class Seat
{
    public int Id { get; set; }
    public string SeatNumber { get; set; } = "";
    public SeatStatus Status { get; set; } = SeatStatus.Available;
    public DateTime? ExpiredTime { get; set; }

    public int? BookingId { get; set; }
}

public class Booking
{
    public int Id { get; set; }
    public string BookingCode { get; set; } = "";
    public string CustomerEmail { get; set; } = "";
    public bool IsPaid { get; set; } = false;

    public List<Seat> Seats { get; set; } = new();
}

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }
    public DbSet<Seat> Seats => Set<Seat>();
    public DbSet<Booking> Bookings => Set<Booking>();
}

public class SePayDto
{
    [JsonPropertyName("content")]
    public string Content { get; set; } = "";
}