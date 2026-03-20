using VehicleBooking.Models.DTOs.Common;
using Models.Dtos.Schedule;

namespace Services.Schedule;

public interface IScheduleService
{
    Task<PagedResult<ScheduleResponse>> GetSchedulesAsync(ScheduleQuery query);
    Task<ScheduleResponse?> GetScheduleByIdAsync(int id);
    Task CreateScheduleAsync(CreateScheduleRequest request);
    Task UpdateScheduleAsync(UpdateScheduleRequest request);
    Task DeleteScheduleAsync(int id);
    Task UpdateScheduleStatusAsync(int id, string status);
    Task<PagedResult<SeatBookingResponse>> GetSeatBookingByScheduleIdAsync(int scheduleId);
}