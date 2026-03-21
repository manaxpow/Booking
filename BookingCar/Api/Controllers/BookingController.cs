using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

[Route("api/[controller]")]
[ApiController]
[Authorize]
public class BookingController(IBookingService bookingService) : ControllerBase
{
    private readonly IBookingService _bookingService = bookingService;

    [HttpPost]
    public async Task<IActionResult> CreateBookingAsync([FromBody] SeatBookingRequest request)
    {
        try
        {
            var response = await _bookingService.CreateBookingAsync(request);
            return Ok(response);
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpGet("{ticketId}/payment-info")]
    public async Task<IActionResult> GetPaymentInfo(int ticketId)
    {
        try
        {
            var response = await _bookingService.GetPaymentDetailsAsync(ticketId);
            return Ok(response);
        }
        catch (Exception ex)
        {
            return NotFound(new { message = ex.Message });
        }
    }

    [HttpPost("sepay-webhook")]
    public async Task<IActionResult> SePayWebhook([FromBody] SePayWebhookDto data)
    {
        Console.WriteLine($"Nhận Webhook từ SePay: Nội dung '{data.content}', Số tiền: {data.transferAmount}");

        var isSuccess = await _bookingService.ConfirmPaymentAsync(data.content, data.transferAmount);

        if (isSuccess)
        {
            return Ok(new { status = "success", message = "Vé đã được xác nhận thanh toán." });
        }

        return BadRequest(new { status = "fail", message = "Nội dung chuyển khoản hoặc số tiền không khớp." });
    }
}