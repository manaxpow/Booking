using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Models.Dtos.Destination;
using VehicleBooking.Models.DTOs;
using VehicleBooking.Models.DTOs.Common;

[ApiController]
[Route("api/[controller]")]
public class DestinationController : ControllerBase
{
    private readonly IDestinationService _destinationService;

    public DestinationController(IDestinationService destinationService)
    {
        _destinationService = destinationService;
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetDestinationById(int id)
    {
        var destination = await _destinationService.GetDestinationByIdAsync(id);
        if (destination == null)
        {
            return NotFound(new ErrorResponse
            {
                Message = "Không tìm thấy điểm đến.",
                StatusCode = 404
            });
        }

        return Ok(new SuccessResponse<DestinationResponse>
        {
            Message = "Lấy điểm đến thành công.",
            Data = destination
        });
    }

    [HttpGet]
    public async Task<IActionResult> GetDestinations([FromQuery] DestinationQuery query)
    {
        var destinations = await _destinationService.GetDestinationsAsync(query);
        return Ok(new SuccessResponse<PagedResult<DestinationResponse>>
        {
            Message = "Lấy danh sách điểm đến thành công.",
            Data = destinations
        });
    }

    [HttpPost]
    [Authorize(Roles = "ADMIN")]
    public async Task<IActionResult> CreateDestination([FromBody] CreateDestinationRequest createDto)
    {
        try
        {
            await _destinationService.CreateDestinationAsync(createDto);
            return Ok(new SuccessResponse<object>
            {
                Message = "Điểm đến được tạo thành công."
            });
        }
        catch (Exception ex)
        {
            return BadRequest(new ErrorResponse
            {
                Message = "Tạo điểm đến thất bại: " + ex.Message,
                StatusCode = 400
            });
        }
    }

    [Authorize(Roles = "ADMIN")]
    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateDestination(int id, [FromBody] UpdateDestinationRequest updateDto)
    {
        try
        {
            await _destinationService.UpdateDestinationAsync(updateDto, id);
            return Ok(new SuccessResponse<object>
            {
                Message = "Điểm đến được cập nhật thành công."
            });
        }
        catch (KeyNotFoundException)
        {
            return NotFound(new ErrorResponse
            {
                Message = "Không tìm thấy điểm đến.",
                StatusCode = 404
            });
        }
        catch (Exception ex)
        {
            return BadRequest(new ErrorResponse
            {
                Message = "Cập nhật điểm đến thất bại: " + ex.Message,
                StatusCode = 400
            });
        }
    }

    [Authorize(Roles = "ADMIN")]
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteDestination(int id)
    {
        try
        {
            await _destinationService.DeleteDestinationAsync(id);
            return Ok(new SuccessResponse<object>
            {
                Message = "Điểm đến đã được xóa thành công."
            });
        }
        catch (KeyNotFoundException)
        {
            return NotFound(new ErrorResponse
            {
                Message = "Không tìm thấy điểm đến.",
                StatusCode = 404
            });
        }
        catch (Exception ex)
        {
            return BadRequest(new ErrorResponse
            {
                Message = "Xóa điểm đến thất bại: " + ex.Message,
                StatusCode = 400
            });
        }
    }
}
