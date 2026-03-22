using Microsoft.EntityFrameworkCore;
using Chapter9;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
var connectionString = "Host=localhost;Port=5432;Database=chapter-9;Username=admin;Password=password123";
builder.Services.AddDbContext<AppDbContext>(opt => opt.UseNpgsql(connectionString));

builder.Services.AddSingleton<IEmailService, EmailService>();

var app = builder.Build();

app.MapControllers();


// Seeding data for test
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();

    db.Database.EnsureCreated();

    // 2. Kiểm tra nếu chưa có dữ liệu thì mới Seed
    if (!db.Bookings.Any())
    {
        // Tạo Đơn hàng mẫu (Id sẽ tự tăng là 1)
        var testBooking = new Booking
        {
            BookingCode = "BK123",
            CustomerEmail = "nguyenthanhnambmt255@gmail.com",
            IsPaid = false
        };

        db.Bookings.Add(testBooking);
        await db.SaveChangesAsync();

        // Tạo Ghế mẫu gắn với đơn hàng trên
        db.Seats.AddRange(
            // Ghế 1: Dùng để test Thanh toán (Chuyển từ Hold -> Sold)
            new Seat
            {
                SeatNumber = "A1",
                Status = SeatStatus.Hold,
                BookingId = testBooking.Id,
                ExpiredTime = DateTime.UtcNow.AddMinutes(30)
            },
            // Ghế 2: Dùng để test CronJob (Sẽ bị dọn dẹp vì hết hạn)
            new Seat
            {
                SeatNumber = "B2",
                Status = SeatStatus.Hold,
                BookingId = testBooking.Id,
                ExpiredTime = DateTime.UtcNow.AddMinutes(-10) // Đã hết hạn 
            }
        );

        await db.SaveChangesAsync();
        Console.WriteLine("[SEED] Đã tạo dữ liệu mẫu thành công!");
    }
}


app.Run();