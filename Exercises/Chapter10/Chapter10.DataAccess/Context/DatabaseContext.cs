using Microsoft.EntityFrameworkCore;
using Chapter10.DataAccess.Entities;

namespace Chapter10.DataAccess.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<SeatBooking> SeatBookings { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<SeatBooking>()
            .HasIndex(sb => new { sb.ScheduleId, sb.IsBooking, sb.IsHold })
            .HasDatabaseName("IX_SeatBooking_Availability");
    }
}