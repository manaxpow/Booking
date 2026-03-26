using Chapter6.DTOs.Schedules;
using Chapter6.Services;
using Chapter6.Services.Common;
using Microsoft.AspNetCore.Mvc;

namespace Chapter6.Controllers;

[ApiController]
[Route("api/[controller]")]
public class SchedulesController : ControllerBase
{
    private readonly IScheduleService _scheduleService;

    public SchedulesController(IScheduleService scheduleService)
    {
        _scheduleService = scheduleService;
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<GetScheduleWithDetailsByIdResponse>> GetById(int id)
    {
        var result = await _scheduleService.GetByIdAsync(id);
        return ToActionResult(result);
    }

    [HttpPost]
    public async Task<ActionResult<CreateScheduleResponse>> Create([FromBody] CreateScheduleRequest request)
    {
        var result = await _scheduleService.CreateScheduleAsync(request);
        if (result.Status == ServiceStatus.Success && result.Data is not null)
        {
            return CreatedAtAction(nameof(GetById), new { id = result.Data.Id }, result.Data);
        }

        return ToActionResult(result);
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id)
    {
        var result = await _scheduleService.DeleteScheduleAsync(id);
        return ToNoContentActionResult(result);
    }

    [HttpPut("{id:int}/status")]
    public async Task<ActionResult<UpdateScheduleStatusResponse>> UpdateStatus(int id, [FromBody] UpdateScheduleStatusRequest request)
    {
        var result = await _scheduleService.UpdateScheduleStatusAsync(id, request);
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

    private IActionResult ToNoContentActionResult(ServiceResult result)
    {
        return result.Status switch
        {
            ServiceStatus.Success => NoContent(),
            ServiceStatus.NotFound => NotFound(result.Message),
            ServiceStatus.BadRequest => BadRequest(result.Message),
            ServiceStatus.Conflict => Conflict(result.Message),
            _ => BadRequest(result.Message ?? "Yêu cầu không hợp lệ.")
        };
    }
}