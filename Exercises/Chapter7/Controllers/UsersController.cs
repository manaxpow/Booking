using System.Security.Claims;
using Chapter6.DTOs.Users;
using Chapter6.Services;
using Chapter6.Services.Common;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Chapter6.Controllers;

[ApiController]
[Authorize]
[Route("api/[controller]")]
public class UsersController : ControllerBase
{
    private readonly IUserService _userService;

    public UsersController(IUserService userService)
    {
        _userService = userService;
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<GetUserByIdResponse>> GetById(int id)
    {
        var isAdmin = User.IsInRole("ADMIN");

        if (!isAdmin)
        {
            var currentUserIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (!int.TryParse(currentUserIdClaim, out var currentUserId))
            {
                return Unauthorized("Token không hợp lệ.");
            }

            if (currentUserId != id)
            {
                return Forbid();
            }
        }

        var result = await _userService.GetByIdAsync(id);
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
