using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Chapter8.Controllers;

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

    [HttpGet("qr/{id}")]
    public async Task<IActionResult> GetPaymentQr(int id)
    {
        var booking = await _context.Bookings.FindAsync(id);
        if (booking == null) return NotFound("Không tìm thấy đơn hàng");


        string bankId = "VPBANK";
        string accountNo = "0328680662";
        string template = "compact2"; // Giao diện QR
        decimal amount = 4000; // Số tiền

        // Nội dung chuyển khoản: Phải khớp với lúc SePay quét Webhook
        string description = booking.BookingCode;

        // Tạo link ảnh từ vietqr.io
        string qrUrl = $"https://img.vietqr.io/image/{bankId}-{accountNo}-{template}.png" +
                       $"?amount={amount}&addInfo={Uri.EscapeDataString(description)}";

        return Ok(new
        {
            bookingCode = booking.BookingCode,
            qrCodeUrl = qrUrl,
            message = "Quét mã QR để hoàn tất thanh toán"
        });
    }

    [HttpPost("sepay-webhook")]
    public async Task<IActionResult> HandleSePay([FromBody] SePayDto data)
    {
        Console.WriteLine(data.Content);
        var booking = await _context.Bookings
        .Include(b => b.Seats)
        .FirstOrDefaultAsync(b => data.Content.ToUpper().Contains(b.BookingCode.ToUpper()));

        if (booking == null) return NotFound("Mã đơn hàng sai");

        // Cập nhật trạng thái thanh toán
        booking.IsPaid = true;
        booking.Seats.ForEach(s => s.Status = SeatStatus.Sold);

        await _context.SaveChangesAsync();

        // Gửi mail thông báo
        await _mail.SendEmailAsync(booking.CustomerEmail, "Xác nhận thanh toán thành công");

        return Ok(new { success = true });
    }

    [HttpGet("status/{bookingId}")]
    public async Task<IActionResult> GetBookingStatus(int bookingId)
    {
        var booking = await _context.Bookings.FindAsync(bookingId);
        if (booking == null) return NotFound("Không tìm thấy đơn hàng");
        return Ok(booking);
    }
}