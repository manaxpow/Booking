using Chapter6.DTOs.Destinations;
using Chapter6.Services;
using Chapter6.Services.Common;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Chapter6.Controllers;

[ApiController]
[Route("api/[controller]")]
public class DestinationsController(IDestinationService destinationService) : ControllerBase
{
    private readonly IDestinationService _destinationService = destinationService;

    [Authorize(Roles = "ADMIN")]
    [HttpPost]
    public async Task<ActionResult<CreateDestinationResponse>> Create(
        [FromBody] CreateDestinationRequest request
    )
    {
        var result = await _destinationService.CreateAsync(request);
        if (result.Status == ServiceStatus.Success && result.Data is not null)
        {
            return CreatedAtAction(nameof(GetById), new { id = result.Data.Id }, result.Data);
        }

        return ToActionResult(result);
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<GetDestinationByIdResponse>> GetById(int id)
    {
        var result = await _destinationService.GetByIdAsync(id);
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
