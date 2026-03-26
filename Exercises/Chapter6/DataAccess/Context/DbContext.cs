using Chapter6.Models;
using Microsoft.EntityFrameworkCore;

namespace Chapter6.DataAccess.Context;

public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
{
	public DbSet<Car> Cars => Set<Car>();
	public DbSet<Schedule> Schedules => Set<Schedule>();
	public DbSet<Driver> Drivers => Set<Driver>();
	public DbSet<Destination> Destinations => Set<Destination>();
	public DbSet<SeatBooking> SeatBookings => Set<SeatBooking>();

	protected override void OnModelCreating(ModelBuilder modelBuilder)
	{
		base.OnModelCreating(modelBuilder);

		modelBuilder.Entity<Car>(entity =>
		{
			entity.HasKey(c => c.Id);
			entity.HasIndex(c => c.LicensePlate).IsUnique();
			entity.Property(c => c.LicensePlate).IsRequired().HasMaxLength(20);
			entity.Property(c => c.Brand).IsRequired().HasMaxLength(100);
			entity.Property(c => c.Model).IsRequired().HasMaxLength(100);
		});

		modelBuilder.Entity<Schedule>(entity =>
		{
			entity.HasKey(s => s.Id);
			entity.Property(s => s.Status).IsRequired().HasMaxLength(20);

			entity.HasOne(s => s.Car)
				  .WithMany(c => c.Schedules)
				  .HasForeignKey(s => s.CarId)
				  .OnDelete(DeleteBehavior.Restrict);

			entity.HasOne(s => s.Driver)
				  .WithMany(d => d.Schedules)
				  .HasForeignKey(s => s.DriverId)
				  .OnDelete(DeleteBehavior.Restrict);

			entity.HasOne(s => s.PickupDestination)
				  .WithMany(d => d.PickupSchedules)
				  .HasForeignKey(s => s.PickupDestinationId)
				  .OnDelete(DeleteBehavior.Restrict);

			entity.HasOne(s => s.DropoffDestination)
				  .WithMany(d => d.DropoffSchedules)
				  .HasForeignKey(s => s.DropoffDestinationId)
				  .OnDelete(DeleteBehavior.Restrict);
		});

		modelBuilder.Entity<Driver>(entity =>
		{
			entity.HasKey(d => d.Id);
			entity.Property(d => d.FullName).IsRequired().HasMaxLength(100);
			entity.HasIndex(d => d.FullName);
		});

		modelBuilder.Entity<Destination>(entity =>
		{
			entity.HasKey(d => d.Id);
			entity.Property(d => d.Name).IsRequired().HasMaxLength(120);
			entity.Property(d => d.Address).HasMaxLength(250);
			entity.HasIndex(d => d.Name);
		});

		modelBuilder.Entity<SeatBooking>(entity =>
		{
			entity.HasKey(sb => sb.Id);
			entity.HasIndex(sb => new { sb.ScheduleId, sb.SeatNumber }).IsUnique();
			entity.HasOne(sb => sb.Schedule)
				  .WithMany(s => s.SeatBookings)
				  .HasForeignKey(sb => sb.ScheduleId)
				  .OnDelete(DeleteBehavior.Cascade);
		});
	}
}
