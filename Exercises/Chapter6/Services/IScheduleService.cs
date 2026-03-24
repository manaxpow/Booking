using Chapter6.DTOs.Schedules;
using Chapter6.Services.Common;

namespace Chapter6.Services;

public interface IScheduleService
{
	Task<ServiceResult<GetPagedSchedulesResponse>> GetPagedSchedulesAsync(GetPagedSchedulesRequest request);
	Task<ServiceResult<GetScheduleWithDetailsByIdResponse>> GetByIdAsync(int id);
	Task<ServiceResult<CreateScheduleResponse>> CreateScheduleAsync(CreateScheduleRequest request);
	Task<ServiceResult> DeleteScheduleAsync(int id);
	Task<ServiceResult<UpdateScheduleStatusResponse>> UpdateScheduleStatusAsync(int id, UpdateScheduleStatusRequest request);
}
