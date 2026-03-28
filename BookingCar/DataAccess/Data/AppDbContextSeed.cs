using DataAccess.Data;
using Microsoft.EntityFrameworkCore;

namespace DataAccess.Data
{
    public static class AppDbContextSeed
    {
        public static async Task SeedData(AppDbContext context)
        {
            // Seed Users
            if (!context.Users.Any())
            {
                await context.Users.AddRangeAsync(
                    new User { FullName = "user1", Phone = "0123456789", Email = "user1@example.com", PasswordHash = "hashed_password_1", Cccd = "123456789012", Role = "USER", CreateAt = DateTime.UtcNow, UpdateAt = DateTime.UtcNow },
                    new User { FullName = "admin1", Phone = "0987654321", Email = "admin1@example.com", PasswordHash = "hashed_password_admin", Cccd = "098765432109", Role = "ADMIN", CreateAt = DateTime.UtcNow, UpdateAt = DateTime.UtcNow }
                );
                await context.SaveChangesAsync();
            }

            // Seed Destinations
            if (!context.Destinations.Any())
            {
                await context.Destinations.AddRangeAsync(
                    new Destination { Province = "Hanoi", CreateAt = DateTime.UtcNow, UpdateAt = DateTime.UtcNow },
                    new Destination { Province = "Ho Chi Minh City", CreateAt = DateTime.UtcNow, UpdateAt = DateTime.UtcNow },
                    new Destination { Province = "Da Nang", CreateAt = DateTime.UtcNow, UpdateAt = DateTime.UtcNow }
                );
                await context.SaveChangesAsync();
            }

            // Seed Cars
            if (!context.Cars.Any())
            {
                await context.Cars.AddRangeAsync(
                    new Car { LicensePlate = "29A-12345", Brand = "Ford Transit", Capacity = 16, CreateAt = DateTime.UtcNow, UpdateAt = DateTime.UtcNow },
                    new Car { LicensePlate = "30B-67890", Brand = "Hyundai Solati", Capacity = 16, CreateAt = DateTime.UtcNow, UpdateAt = DateTime.UtcNow },
                    new Car { LicensePlate = "51C-11223", Brand = "Thaco Universe", Capacity = 45, CreateAt = DateTime.UtcNow, UpdateAt = DateTime.UtcNow }
                );
                await context.SaveChangesAsync();
            }

            // Seed Seats for Cars
            if (!context.Seats.Any())
            {
                var cars = context.Cars.ToList();
                var seats = new List<Seat>();
                foreach (var car in cars)
                {
                    for (int i = 1; i <= car.Capacity; i++)
                    {
                        seats.Add(new Seat { CarId = car.Id, Name = $"A{i}", CreateAt = DateTime.UtcNow, UpdateAt = DateTime.UtcNow });
                    }
                }
                await context.Seats.AddRangeAsync(seats);
                await context.SaveChangesAsync();
            }

            // Seed Drivers
            if (!context.Drivers.Any())
            {
                await context.Drivers.AddRangeAsync(
                    new Driver { Name = "Nguyen Van A", Dob = DateTime.Parse("1980-01-01"), License = "DRV001", CreateAt = DateTime.UtcNow, UpdateAt = DateTime.UtcNow },
                    new Driver { Name = "Tran Thi B", Dob = DateTime.Parse("1985-05-10"), License = "DRV002", CreateAt = DateTime.UtcNow, UpdateAt = DateTime.UtcNow }
                );
                await context.SaveChangesAsync();
            }

            // Seed Schedules
            if (!context.Schedules.Any())
            {
                var hanoi = context.Destinations.FirstOrDefault(d => d.Province == "Hanoi");
                var hcmc = context.Destinations.FirstOrDefault(d => d.Province == "Ho Chi Minh City");
                var danang = context.Destinations.FirstOrDefault(d => d.Province == "Da Nang");
                var car1 = context.Cars.FirstOrDefault(c => c.LicensePlate == "29A-12345");
                var car2 = context.Cars.FirstOrDefault(c => c.LicensePlate == "30B-67890");
                var driver1 = context.Drivers.FirstOrDefault(d => d.License == "DRV001");
                var driver2 = context.Drivers.FirstOrDefault(d => d.License == "DRV002");

                if (hanoi != null && hcmc != null && danang != null && car1 != null && car2 != null && driver1 != null && driver2 != null)
                {
                    await context.Schedules.AddRangeAsync(
                        new Schedule
                        {
                            CarId = car1.Id,
                            DriverId = driver1.Id,
                            FromDestinationId = hanoi.Id,
                            ToDestinationId = hcmc.Id,
                            StartTime = DateTime.UtcNow.AddDays(1),
                            EndTime = DateTime.UtcNow.AddDays(1).AddHours(2),
                            ExpectedDuration = TimeSpan.FromHours(2),
                            Status = "ACTIVE",
                            Price = 500000,
                            CreateAt = DateTime.UtcNow,
                            UpdateAt = DateTime.UtcNow
                        },
                        new Schedule
                        {
                            CarId = car2.Id,
                            DriverId = driver2.Id,
                            FromDestinationId = hcmc.Id,
                            ToDestinationId = danang.Id,
                            StartTime = DateTime.UtcNow.AddDays(2),
                            EndTime = DateTime.UtcNow.AddDays(2).AddHours(3),
                            ExpectedDuration = TimeSpan.FromHours(3),
                            Status = "ACTIVE",
                            Price = 300000,
                            CreateAt = DateTime.UtcNow,
                            UpdateAt = DateTime.UtcNow
                        }
                    );
                    await context.SaveChangesAsync();
                }
            }

            // Seed SeatBookings (some booked, some available)
            if (!context.SeatBookings.Any())
            {
                var schedule1 = context.Schedules.FirstOrDefault();
                if (schedule1 != null)
                {
                    var seatsForCar1 = context.Seats.Where(s => s.CarId == schedule1.CarId).Take(2).ToList();

                    if (seatsForCar1.Count >= 2)
                    {
                        await context.SeatBookings.AddRangeAsync(
                            new SeatBooking { ScheduleId = schedule1.Id, SeatId = seatsForCar1[0].Id, IsHold = true, IsBooking = false, CreateAt = DateTime.UtcNow, UpdateAt = DateTime.UtcNow },
                            new SeatBooking { ScheduleId = schedule1.Id, SeatId = seatsForCar1[1].Id, IsHold = false, IsBooking = false, CreateAt = DateTime.UtcNow, UpdateAt = DateTime.UtcNow }
                        );
                        await context.SaveChangesAsync();
                        // No longer decrement AvailableSeats as it's not a property of Schedule
                    }
                }
            }

            // Seed Tickets and TicketDetails and Payments
            if (!context.Tickets.Any())
            {
                var user1 = context.Users.FirstOrDefault(u => u.FullName == "user1");
                var seatBooking1 = context.SeatBookings.FirstOrDefault(sb => sb.IsHold == true);

                if (user1 != null && seatBooking1 != null)
                {
                    var newTicket = new Ticket
                    {
                        UserId = user1.Id,
                        Status = TicketStatus.CONFIRMED,
                        TotalPrice = 500000,
                        CreateAt = DateTime.UtcNow,
                        UpdateAt = DateTime.UtcNow
                    };
                    await context.Tickets.AddAsync(newTicket);
                    await context.SaveChangesAsync();

                    await context.TicketDetails.AddAsync(
                        new TicketDetail { TicketId = newTicket.Id, SeatBookingId = seatBooking1.Id }
                    );
                    await context.SaveChangesAsync();

                    await context.Payments.AddAsync(
                        new Payment { TicketId = newTicket.Id, Amount = 500000, OrderCode = "ORDER123", Status = PaymentStatus.PAID, CreatedAt = DateTime.UtcNow, ExpiredAt = DateTime.UtcNow.AddMinutes(10) }
                    );
                    await context.SaveChangesAsync();
                }
            }
        }
    }
}
