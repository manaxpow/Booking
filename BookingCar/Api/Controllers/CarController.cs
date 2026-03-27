using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OutputCaching;
using VehicleBooking.Models.DTOs.Car;
using VehicleBooking.Models.DTOs.Common;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class CarController : ControllerBase
{
    private readonly ICarService _carService;

    public CarController(ICarService carService)
    {
        _carService = carService;
    }

    [HttpGet]
    [AllowAnonymous]
    [OutputCache(PolicyName = "CarSearchResult")]
    public async Task<IActionResult> GetCars([FromQuery] CarQueryParameters query)
    {
        var result = await _carService.GetCarsAsync(query);
        return Ok(result);
    }

    [HttpGet("{id}")]
    [AllowAnonymous]
    public async Task<IActionResult> GetCarDetail(int id)
    {
        var car = await _carService.GetCarByIdAsync(id);
        if (car == null)
        {
            return NotFound("Không tìm thấy xe.");
        }
        return Ok(car);
    }

    [HttpPost]
    [Authorize(Roles = "ADMIN")] // Chỉ những người có Role là "ADMIN" mới gọi được
    public async Task<IActionResult> AddCar([FromBody] CreateCarRequest request)
    {
        try
        {
            await _carService.CreateCarAsync(request);
            return Ok("Admin đã thêm xe thành công");
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpPut("{id}")]
    [Authorize(Roles = "ADMIN")]
    public async Task<IActionResult> UpdateCar(int id, [FromBody] UpdateCarRequest request)
    {
        if (id != request.Id)
        {
            return BadRequest("ID in route does not match ID in request body.");
        }

        try
        {
            await _carService.UpdateCarAsync(request);
            return Ok("Cập nhật thông tin xe thành công");
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpDelete("{id}")]
    [Authorize(Roles = "ADMIN")]
    public async Task<IActionResult> DeleteCar(int id)
    {
        try
        {
            await _carService.DeleteCarAsync(id);
            return Ok("Xóa xe thành công");
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }
}