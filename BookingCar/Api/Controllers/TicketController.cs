using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using VehicleBooking.Models.DTOs;
using VehicleBooking.Models.DTOs.Common;
using Services.Ticket;
using System.Security.Claims;
using Models.Dtos.Ticket;

[Route("api/[controller]")]
[ApiController]
[Authorize]
public class TicketController(ITicketService ticketService) : ControllerBase
{
    private readonly ITicketService _ticketService = ticketService;
    [HttpGet("user")]
    public async Task<IActionResult> GetUserTickets([FromQuery] int page = 1, [FromQuery] int pageSize = 10)
    {
        try
        {
            // Get user ID from claims
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userIdClaim) || !int.TryParse(userIdClaim, out int userId))
            {
                return Unauthorized(new ErrorResponse
                {
                    StatusCode = 401,
                    Message = "Không xác định được người dùng."
                });
            }

            var tickets = await _ticketService.GetTicketsByUserIdAsync(userId, page, pageSize);
            return Ok(new SuccessResponse<PagedResult<Models.Dtos.Ticket.TicketResponse>>
            {
                Message = "Lấy danh sách vé thành công.",
                Data = tickets
            });
        }
        catch (Exception ex)
        {
            return BadRequest(new ErrorResponse
            {
                StatusCode = 400,
                Message = "Lỗi khi lấy danh sách vé: " + ex.Message
            });
        }
    }


    [HttpGet("{id}")]
    public async Task<IActionResult> GetTicketById(int id)
    {
        try
        {
            // Get user ID from claims
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (string.IsNullOrEmpty(userIdClaim) || !int.TryParse(userIdClaim, out int userId))
            {
                return Unauthorized(new ErrorResponse
                {
                    StatusCode = 401,
                    Message = "Không xác định được người dùng."
                });
            }

            var ticket = await _ticketService.GetTicketByIdAsync(id);

            if (ticket == null)
            {
                return NotFound(new ErrorResponse
                {
                    StatusCode = 404,
                    Message = "Không tìm thấy vé."
                });
            }

            // Check if the ticket belongs to the current user
            if (ticket.UserId != userId)
            {
                return Forbid();
            }

            return Ok(new SuccessResponse<TicketDetailResponse>
            {
                Message = "Lấy thông tin vé thành công.",
                Data = ticket
            });
        }
        catch (Exception ex)
        {
            return BadRequest(new ErrorResponse
            {
                StatusCode = 400,
                Message = "Lỗi khi lấy thông tin vé: " + ex.Message
            });
        }
    }

    [HttpGet("{id}/details")]
    public async Task<IActionResult> GetTicketDetails(int id)
    {
        try
        {
            // Get user ID from claims
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (string.IsNullOrEmpty(userIdClaim) || !int.TryParse(userIdClaim, out int userId))
            {
                return Unauthorized(new ErrorResponse
                {
                    StatusCode = 401,
                    Message = "Không xác định được người dùng."
                });
            }

            var ticket = await _ticketService.GetTicketByIdAsync(id);

            if (ticket == null)
            {
                return NotFound(new ErrorResponse
                {
                    StatusCode = 404,
                    Message = "Không tìm thấy vé."
                });
            }

            // Check if the ticket belongs to the current user
            if (ticket.UserId != userId)
            {
                return Forbid();
            }

            var details = await _ticketService.GetTicketDetailsAsync(id);

            return Ok(new SuccessResponse<List<Models.Dtos.Ticket.TicketResponse>>
            {
                Message = "Lấy chi tiết vé thành công.",
                Data = details
            });
        }
        catch (Exception ex)
        {
            return BadRequest(new ErrorResponse
            {
                StatusCode = 400,
                Message = "Lỗi khi lấy chi tiết vé: " + ex.Message
            });
        }
    }

}