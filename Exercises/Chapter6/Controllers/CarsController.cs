using Chapter6.DTOs.Cars;
using Chapter6.Services;
using Chapter6.Services.Common;
using Microsoft.AspNetCore.Mvc;

namespace Chapter6.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CarsController : ControllerBase
{
    private readonly ICarService _carService;

    public CarsController(ICarService carService)
    {
        _carService = carService;
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<GetCarByIdResponse>> GetById(int id)
    {
        var result = await _carService.GetByIdAsync(id);
        return ToActionResult(result);
    }

    [HttpPost]
    public async Task<ActionResult<CreateCarResponse>> Create([FromBody] CreateCarRequest request)
    {
        var result = await _carService.CreateAsync(request);
        if (result.Status == ServiceStatus.Success && result.Data is not null)
        {
            return CreatedAtAction(nameof(GetById), new { id = result.Data.Id }, result.Data);
        }

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
            _ => BadRequest(result.Message ?? "Yêu cầu không hợp lệ.")
        };
    }
}