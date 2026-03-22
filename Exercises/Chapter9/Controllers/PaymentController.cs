using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Chapter9.Controllers;

[ApiController]
[Route("api/[controller]")]
public class PaymentController : ControllerBase
{
    private readonly AppDbContext _context;
    private readonly IEmailService _mail;

    public PaymentController(AppDbContext context, IEmailService mail)
    {
        _context = context;
        _mail = mail;
    }

    [HttpPost("sepay-webhook")]
    public async Task<IActionResult> HandleSePay([FromBody] SePayDto data)
    {
        // Spin up transaction
        using var transaction = await _context.Database.BeginTransactionAsync();
        try
        {
            // Using raw SQL to lock row
            var booking = await _context.Bookings
                .FromSqlRaw("SELECT * FROM \"Bookings\" WHERE {0} ILIKE '%' || \"BookingCode\" || '%' FOR UPDATE", data.Content)
                .Include(b => b.Seats)
                .FirstOrDefaultAsync();

            if (booking == null) return NotFound();
            Console.WriteLine(booking.Id + " " + booking.BookingCode);
            if (booking.IsPaid) return Ok(new { msg = "Đơn hàng đã thanh toán trước đó." });

            // 3. Tiến hành khóa ghế và đợi
            var seatIds = booking.Seats.Select(s => s.Id).ToList();
            var lockedSeats = await _context.Seats
                .FromSqlRaw("SELECT * FROM \"Seats\" WHERE \"Id\" = ANY({0}) FOR UPDATE", seatIds)
                .ToListAsync();

            Console.WriteLine($"[LOCK] Đã khóa {lockedSeats.Count} ghế. Đang treo để test...");

            await Task.Delay(20000);
            booking.IsPaid = true;
            foreach (var seat in booking.Seats)
            {
                seat.Status = SeatStatus.Sold;
            }
            await _context.SaveChangesAsync();

            await transaction.CommitAsync();

            // Gửi email thông báo (Thực hiện sau khi DB đã an toàn)
            await _mail.SendEmailAsync(booking.CustomerEmail, "Xác nhận thanh toán thành công");

            return Ok(new { success = true, message = "Giao dịch hoàn tất an toàn." });
        }
        catch (Exception ex)
        {
            await transaction.RollbackAsync();
            Console.WriteLine($"[TRANSACTION ERROR] Đã Rollback dữ liệu. Chi tiết: {ex.Message}");
            return StatusCode(500, "Lỗi xử lý giao dịch.");
        }
    }
}