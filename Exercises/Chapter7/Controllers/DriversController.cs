using Chapter6.DTOs.Drivers;
using Chapter6.Services.Common;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Chapter6.Controllers;

[ApiController]
[Route("api/[controller]")]
public class DriversController : ControllerBase
{
    private readonly IDriverService _driverService;

    public DriversController(IDriverService driverService)
    {
        _driverService = driverService;
    }

    [AllowAnonymous]
    [HttpGet]
    public async Task<ActionResult<IEnumerable<GetDriverByIdResponse>>> GetAll()
    {
        var result = await _driverService.GetAllAsync();
        return ToActionResult(result);
    }

    [Authorize(Roles = "ADMIN")]
    [HttpPost]
    public async Task<ActionResult<CreateDriverResponse>> Create(
        [FromBody] CreateDriverRequest request
    )
    {
        var result = await _driverService.CreateAsync(request);
        if (result.Status == ServiceStatus.Success && result.Data is not null)
        {
            return CreatedAtAction(nameof(GetById), new { id = result.Data.Id }, result.Data);
        }

        return ToActionResult(result);
    }

    [AllowAnonymous]
    [HttpGet("{id:int}")]
    public async Task<ActionResult<GetDriverByIdResponse>> GetById(int id)
    {
        var result = await _driverService.GetByIdAsync(id);
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
