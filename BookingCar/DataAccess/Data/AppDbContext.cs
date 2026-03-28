using Microsoft.EntityFrameworkCore;

namespace DataAccess.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<User> Users { get; set; }
        public DbSet<Car> Cars { get; set; }
        public DbSet<Seat> Seats { get; set; }
        public DbSet<Driver> Drivers { get; set; }
        public DbSet<Destination> Destinations { get; set; }
        public DbSet<Schedule> Schedules { get; set; }
        public DbSet<SeatBooking> SeatBookings { get; set; }
        public DbSet<Ticket> Tickets { get; set; }
        public DbSet<Payment> Payments { get; set; }
        public DbSet<TicketDetail> TicketDetails { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Car - Seat (1-n)
            modelBuilder.Entity<Seat>()
                .HasOne(s => s.Car)
                .WithMany(c => c.Seats)
                .HasForeignKey(s => s.CarId);

            // Schedule Config
            modelBuilder.Entity<Schedule>(entity =>
            {
                entity.Property(e => e.Price).HasPrecision(12, 2);

                entity.HasOne(s => s.Car)
                    .WithMany(c => c.Schedules)
                    .HasForeignKey(s => s.CarId);

                entity.HasOne(s => s.Driver)
                    .WithMany(d => d.Schedules)
                    .HasForeignKey(s => s.DriverId);

                // Quan hệ Destination (From/To)
                entity.HasOne(s => s.FromDestination)
                    .WithMany(d => d.FromSchedules)
                    .HasForeignKey(s => s.FromDestinationId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(s => s.ToDestination)
                    .WithMany(d => d.ToSchedules)
                    .HasForeignKey(s => s.ToDestinationId)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            // SeatBooking Config
            modelBuilder.Entity<SeatBooking>(entity =>
            {
                entity.HasOne(sb => sb.Schedule)
                    .WithMany(s => s.SeatBookings)
                    .HasForeignKey(sb => sb.ScheduleId);

                entity.HasOne(sb => sb.Seat)
                    .WithMany(s => s.SeatBookings)
                    .HasForeignKey(sb => sb.SeatId);
            });

            // Ticket Config
            modelBuilder.Entity<Ticket>(entity =>
            {
                entity.HasIndex(t => t.UserId);
                entity.HasIndex(t => t.Status);
                entity.HasIndex(t => t.CreateAt);

                entity.HasOne(t => t.User)
                    .WithMany(u => u.Tickets)
                    .HasForeignKey(t => t.UserId)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            // TicketDetail Config
            modelBuilder.Entity<TicketDetail>(entity =>
            {
                entity.HasIndex(td => td.TicketId);
                entity.HasIndex(td => td.SeatBookingId);

                entity.HasOne(td => td.Ticket)
                    .WithMany(td => td.TicketDetails)
                    .HasForeignKey(td => td.TicketId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(td => td.SeatBooking)
                    .WithMany()
                    .HasForeignKey(td => td.SeatBookingId)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            // Payment Config
            modelBuilder.Entity<Payment>(entity =>
            {
                entity.Property(e => e.Amount).HasPrecision(12, 2);

                entity.HasOne(p => p.Ticket)
                    .WithMany()
                    .HasForeignKey(p => p.TicketId)
                    .OnDelete(DeleteBehavior.Cascade);
            });
        }
    }
}