using System.Security.Claims;
using Chapter6.DTOs.Tickets;
using Chapter6.Services;
using Chapter6.Services.Common;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Chapter6.Controllers;

[ApiController]
[Authorize]
[Route("api/[controller]")]
public class TicketsController : ControllerBase
{
    private readonly ITicketService _ticketService;

    public TicketsController(ITicketService ticketService)
    {
        _ticketService = ticketService;
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<GetTicketByIdResponse>> GetById(int id)
    {
        var isAdmin = User.IsInRole("ADMIN");

        var currentUserIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);

        if (!int.TryParse(currentUserIdClaim, out var currentUserId))
        {
            return Unauthorized("Token không hợp lệ.");
        }

        var result = await _ticketService.GetByIdForUserAsync(id, currentUserId, isAdmin);
        return ToActionResult(result);
    }

    private ActionResult<T> ToActionResult<T>(ServiceResult<T> result)
    {
        return result.Status switch
        {
            ServiceStatus.Success when result.Data is not null => Ok(result.Data),
            ServiceStatus.NotFound => NotFound(result.Message),
            ServiceStatus.BadRequest => BadRequest(result.Message),
            ServiceStatus.Conflict => Conflict(result.Message),
            _ => BadRequest(result.Message ?? "Yêu cầu không hợp lệ."),
        };
    }
}
