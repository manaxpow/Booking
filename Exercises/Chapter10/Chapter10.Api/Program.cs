using Chapter10.DataAccess.Data;
using Chapter10.DataAccess.Entities;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Đăng ký DB (PostgreSQL)
var connectionString = "Host=localhost;Port=5432;Database=chapter-10;Username=admin;Password=password123";
builder.Services.AddDbContext<AppDbContext>(opt => opt.UseNpgsql(connectionString));

// Đăng ký Redis Cache
builder.Services.AddStackExchangeRedisCache(options =>
{
    options.Configuration = "localhost:6379";
    options.InstanceName = "Chapter10_";
});

builder.Services.AddControllers();
var app = builder.Build();

app.MapControllers();

// Seed data
using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    context.Database.EnsureCreated();

    if (!context.SeatBookings.Any())
    {
        var mockSeats = Enumerable.Range(1, 10).Select(i => new SeatBooking
        {
            ScheduleId = 1,
            SeatNumber = $"A{i}",
            IsBooking = false,
            IsHold = false
        }).ToList();

        context.SeatBookings.AddRange(mockSeats);
        context.SaveChanges();
        Console.WriteLine("---> Đã nạp 10 ghế mẫu cho ScheduleId = 1");
    }
}
app.Run();