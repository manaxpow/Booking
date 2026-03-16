using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/[controller]")]
public class DriverController : ControllerBase
{
    private readonly IDriverService _driverService;

    public DriverController(IDriverService driverService)
    {
        _driverService = driverService;
    }

    [HttpGet]
    [AllowAnonymous]
    public async Task<IActionResult> GetDrivers([FromQuery] DriverQueryParameters query)
    {
        var result = await _driverService.GetDriversAsync(query);
        return Ok(result);
    }

    [HttpGet("{id}")]
    [AllowAnonymous]
    public async Task<IActionResult> GetDriverById(int id)
    {
        var driver = await _driverService.GetDriverByIdAsync(id);
        if (driver == null)
        {
            return NotFound("Driver not found.");
        }

        return Ok(driver);
    }

    [HttpPost]
    [Authorize(Roles = "ADMIN")]
    public async Task<IActionResult> CreateDriver([FromBody] CreateDriverRequest request)
    {
        try
        {
            await _driverService.CreateDriverAsync(request);
            return Ok("Create driver successfully.");
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpPut("{id}")]
    [Authorize(Roles = "ADMIN")]
    public async Task<IActionResult> UpdateDriver(int id, [FromBody] UpdateDriverRequest request)
    {
        if (id != request.Id)
        {
            return BadRequest("ID in route does not match ID in request body.");
        }

        try
        {
            await _driverService.UpdateDriverAsync(request);
            return Ok("Update driver successfully.");
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpDelete("{id}")]
    [Authorize(Roles = "ADMIN")]
    public async Task<IActionResult> DeleteDriver(int id)
    {
        try
        {
            await _driverService.DeleteDriverAsync(id);
            return Ok("Delete driver successfully.");
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }
}
