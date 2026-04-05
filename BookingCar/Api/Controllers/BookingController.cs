using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using VehicleBooking.Models.DTOs;

[Route("api/[controller]")]
[ApiController]
[Authorize]
public class BookingController(IBookingService bookingService) : ControllerBase
{
    private readonly IBookingService _bookingService = bookingService;

    [HttpPost]
    [Authorize(Roles = "USER")]
    public async Task<IActionResult> CreateBookingAsync([FromBody] SeatBookingRequest request)
    {
        try
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (string.IsNullOrEmpty(userIdClaim) || !int.TryParse(userIdClaim, out int userId))
            {
                return Unauthorized(new ErrorResponse
                {
                    StatusCode = 401,
                    Message = "Không xác định được người dùng."
                });
            }
            if (userId != request.UserId)
            {
                return Forbid();
            }
            ;

            var response = await _bookingService.CreateBookingAsync(request);
            return Ok(response);
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpGet("{paymentId}/payment-info")]
    public async Task<IActionResult> GetPaymentInfo(int paymentId)
    {
        try
        {
            var response = await _bookingService.GetPaymentDetailsAsync(paymentId);
            return Ok(response);
        }
        catch (Exception ex)
        {
            return NotFound(new { message = ex.Message });
        }
    }

    [HttpPost("sepay-webhook")]
    [AllowAnonymous]
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