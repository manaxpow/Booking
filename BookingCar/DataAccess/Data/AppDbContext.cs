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

            // --- USER INDEXES ---
            modelBuilder.Entity<User>(entity =>
            {
                entity.HasIndex(u => u.Email).IsUnique();
                entity.HasIndex(u => u.Phone).IsUnique();
            });

            // --- CAR & SEAT INDEXES ---
            modelBuilder.Entity<Seat>()
                .HasOne(s => s.Car)
                .WithMany(c => c.Seats)
                .HasForeignKey(s => s.CarId);

            modelBuilder.Entity<Seat>().HasIndex(s => s.CarId);

            // --- SCHEDULE CONFIG & INDEXES ---
            modelBuilder.Entity<Schedule>(entity =>
            {
                entity.Property(e => e.Price).HasPrecision(12, 2);

                // Index cho việc tìm chuyến xe theo ngày và địa điểm
                entity.HasIndex(s => new { s.StartTime, s.FromDestinationId, s.ToDestinationId });
                entity.HasIndex(s => s.CarId);
                entity.HasIndex(s => s.DriverId);

                entity.HasOne(s => s.Car)
                    .WithMany(c => c.Schedules)
                    .HasForeignKey(s => s.CarId);

                entity.HasOne(s => s.Driver)
                    .WithMany(d => d.Schedules)
                    .HasForeignKey(s => s.DriverId);

                entity.HasOne(s => s.FromDestination)
                    .WithMany(d => d.FromSchedules)
                    .HasForeignKey(s => s.FromDestinationId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(s => s.ToDestination)
                    .WithMany(d => d.ToSchedules)
                    .HasForeignKey(s => s.ToDestinationId)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            // --- SEATBOOKING CONFIG & INDEXES ---
            modelBuilder.Entity<SeatBooking>(entity =>
            {
                // UNIQUE INDEX: Quan trọng - Ngăn chặn đặt trùng 1 ghế trên cùng 1 chuyến
                entity.HasIndex(sb => new { sb.ScheduleId, sb.SeatId }).IsUnique();

                entity.HasOne(sb => sb.Schedule)
                    .WithMany(s => s.SeatBookings)
                    .HasForeignKey(sb => sb.ScheduleId);

                entity.HasOne(sb => sb.Seat)
                    .WithMany(s => s.SeatBookings)
                    .HasForeignKey(sb => sb.SeatId);
            });

            // --- TICKET CONFIG & INDEXES ---
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

            // --- TICKETDETAIL CONFIG & INDEXES ---
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

            // --- PAYMENT CONFIG & INDEXES ---
            modelBuilder.Entity<Payment>(entity =>
            {
                entity.Property(e => e.Amount).HasPrecision(12, 2);
                entity.HasIndex(p => p.TicketId);
                entity.HasIndex(p => p.Status);

                entity.HasOne(p => p.Ticket)
                    .WithMany()
                    .HasForeignKey(p => p.TicketId)
                    .OnDelete(DeleteBehavior.Cascade);
            });
        }
    }
}