using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Models.Dtos.Schedule;
using Services.Schedule;
using VehicleBooking.Models.DTOs;
using VehicleBooking.Models.DTOs.Common;

[ApiController]
[Route("api/[controller]")]
public class ScheduleController : ControllerBase
{
    private readonly IScheduleService _scheduleService;

    public ScheduleController(IScheduleService scheduleService)
    {
        _scheduleService = scheduleService;
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetScheduleById(int id)
    {
        if (id <= 0)
        {
            return BadRequest(new ErrorResponse
            {
                StatusCode = 400,
                Message = "ID không hợp lệ."
            });
        }
        var schedule = await _scheduleService.GetScheduleByIdAsync(id);
        if (schedule == null)
        {
            return NotFound(new ErrorResponse
            {
                StatusCode = 404,
                Message = "Không tìm thấy lịch trình."
            });
        }
        return Ok(new SuccessResponse<ScheduleResponse>
        {
            Message = "Lấy lịch trình thành công.",
            Data = schedule
        });
    }

    [HttpGet]
    public async Task<IActionResult> GetSchedules([FromQuery] ScheduleQuery query)
    {
        var schedules = await _scheduleService.GetSchedulesAsync(query);
        var pagedResult = new PagedResult<ScheduleResponse>
        {
            Items = schedules.Items,
            TotalCount = schedules.TotalCount,
            Page = schedules.Page,
            PageSize = schedules.PageSize
        };
        return Ok(new SuccessResponse<PagedResult<ScheduleResponse>>
        {
            Message = "Lấy danh sách lịch trình thành công.",
            Data = pagedResult
        });
    }

    [HttpPost]
    [Authorize(Roles = "ADMIN")]
    public async Task<IActionResult> CreateSchedule([FromBody] CreateScheduleRequest createDto)
    {
        try
        {
            await _scheduleService.CreateScheduleAsync(createDto);
            return Ok(new SuccessResponse<object>
            {
                Message = "Lịch trình được tạo thành công."
            });
        }
        catch (Exception ex)
        {
            return BadRequest(new ErrorResponse
            {
                StatusCode = 400,
                Message = "Tạo lịch trình thất bại: " + ex.Message
            });
        }
    }

    [Authorize(Roles = "ADMIN")]
    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateSchedule(int id, [FromBody] UpdateScheduleRequest updateDto)
    {
        try
        {
            await _scheduleService.UpdateScheduleAsync(updateDto with { Id = id });
            return Ok(new SuccessResponse<object>
            {
                Message = "Lịch trình được cập nhật thành công."
            });
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new ErrorResponse
            {
                StatusCode = 404,
                Message = "Cập nhật lịch trình thất bại: " + ex.Message
            });
        }
        catch (Exception ex)
        {
            return BadRequest(new ErrorResponse
            {
                StatusCode = 400,
                Message = "Cập nhật lịch trình thất bại: " + ex.Message
            });
        }
    }

    [Authorize(Roles = "ADMIN")]
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteSchedule(int id)
    {
        try
        {
            await _scheduleService.DeleteScheduleAsync(id);
            return Ok("Lịch trình đã được xóa thành công.");
        }
        catch (KeyNotFoundException)
        {
            return NotFound("Không tìm thấy lịch trình.");
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [Authorize(Roles = "ADMIN")]
    [HttpPatch("{id}/status")]
    public async Task<IActionResult> UpdateScheduleStatus(int id, [FromBody] string status)
    {
        try
        {
            await _scheduleService.UpdateScheduleStatusAsync(id, status);
            return Ok(new SuccessResponse<object>
            {
                Message = "Trạng thái lịch trình đã được cập nhật thành công."
            });
        }
        catch (KeyNotFoundException)
        {
            return NotFound(new ErrorResponse
            {
                StatusCode = 404,
                Message = "Không tìm thấy lịch trình."
            });
        }
        catch (Exception ex)
        {
            return BadRequest(new ErrorResponse
            {
                StatusCode = 400,
                Message = "Cập nhật trạng thái lịch trình thất bại: " + ex.Message
            });
        }
    }
}