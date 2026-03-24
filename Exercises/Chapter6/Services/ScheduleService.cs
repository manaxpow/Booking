using Chapter6.DataAccess.UnitOfWork;
using Chapter6.DTOs.Schedules;
using Chapter6.Models;
using Chapter6.Services.Common;

namespace Chapter6.Services;

public class ScheduleService : IScheduleService
{
	private readonly IUnitOfWork _unitOfWork;
	private static readonly HashSet<string> AllowedStatuses = ["ACTIVE", "INACTIVE", "FINISH"];

	public ScheduleService(IUnitOfWork unitOfWork)
	{
		_unitOfWork = unitOfWork;
	}

	public async Task<ServiceResult<GetPagedSchedulesResponse>> GetPagedSchedulesAsync(GetPagedSchedulesRequest request)
	{
		if (request.Page <= 0 || request.PageSize <= 0)
		{
			return ServiceResult<GetPagedSchedulesResponse>.BadRequest("page và pageSize phải lớn hơn 0.");
		}

		var fromDateUtc = EnsureUtc(request.FromDate);
		var toDateUtc = EnsureUtc(request.ToDate);

		var schedules = await _unitOfWork.Schedules.GetPagedSchedulesAsync(request.DriverName, fromDateUtc, toDateUtc, request.Page, request.PageSize);
		var items = schedules.Select(MapScheduleSummaryResponse).ToList();

		return ServiceResult<GetPagedSchedulesResponse>.Success(new GetPagedSchedulesResponse
		{
			Page = request.Page,
			PageSize = request.PageSize,
			Count = items.Count,
			Items = items
		});
	}

	public async Task<ServiceResult<GetScheduleWithDetailsByIdResponse>> GetByIdAsync(int id)
	{
		var schedule = await _unitOfWork.Schedules.GetScheduleWithDetailsByIdAsync(id);
		if (schedule is null)
		{
			return ServiceResult<GetScheduleWithDetailsByIdResponse>.NotFound($"Không tìm thấy lịch chạy id={id}.");
		}

		var summary = MapScheduleSummaryResponse(schedule);
		return ServiceResult<GetScheduleWithDetailsByIdResponse>.Success(new GetScheduleWithDetailsByIdResponse
		{
			Id = summary.Id,
			CarId = summary.CarId,
			DriverId = summary.DriverId,
			DriverName = summary.DriverName,
			PickupDestinationId = summary.PickupDestinationId,
			PickupDestinationName = summary.PickupDestinationName,
			DropoffDestinationId = summary.DropoffDestinationId,
			DropoffDestinationName = summary.DropoffDestinationName,
			StartTime = summary.StartTime,
			EndTime = summary.EndTime,
			IsActive = summary.IsActive,
			Status = summary.Status,
			TotalSeatBookings = summary.TotalSeatBookings,
			TotalBookedSeats = summary.TotalBookedSeats,
			Car = summary.Car
		});
	}

	public async Task<ServiceResult<CreateScheduleResponse>> CreateScheduleAsync(CreateScheduleRequest request)
	{
		var startTimeUtc = EnsureUtc(request.StartTime);
		var endTimeUtc = EnsureUtc(request.EndTime);

		if (endTimeUtc <= startTimeUtc)
		{
			return ServiceResult<CreateScheduleResponse>.BadRequest("EndTime phải lớn hơn StartTime.");
		}

		var car = await _unitOfWork.Cars.GetByIdAsync(request.CarId);
		if (car is null)
		{
			return ServiceResult<CreateScheduleResponse>.BadRequest($"Xe id={request.CarId} không tồn tại.");
		}

		var driver = await _unitOfWork.Drivers.GetByIdAsync(request.DriverId);
		if (driver is null)
		{
			return ServiceResult<CreateScheduleResponse>.BadRequest($"Tài xế id={request.DriverId} không tồn tại.");
		}

		var pickupDestination = await _unitOfWork.Destinations.GetByIdAsync(request.PickupDestinationId);
		if (pickupDestination is null)
		{
			return ServiceResult<CreateScheduleResponse>.BadRequest($"Điểm đón id={request.PickupDestinationId} không tồn tại.");
		}

		var dropoffDestination = await _unitOfWork.Destinations.GetByIdAsync(request.DropoffDestinationId);
		if (dropoffDestination is null)
		{
			return ServiceResult<CreateScheduleResponse>.BadRequest($"Điểm trả id={request.DropoffDestinationId} không tồn tại.");
		}

		var hasConflict = await HasScheduleConflictAsync(request);
		if (hasConflict)
		{
			return ServiceResult<CreateScheduleResponse>.Conflict("Schedule bị conflict với lịch hiện tại. (BT8.1 sẽ triển khai chi tiết)");
		}

		var carWithSeats = await _unitOfWork.Cars.GetCarWithSeatsByIdAsync(request.CarId);
		if (carWithSeats is null)
		{
			return ServiceResult<CreateScheduleResponse>.BadRequest($"Không lấy được thông tin ghế của xe id={request.CarId}.");
		}

		var schedule = new Schedule
		{
			CarId = request.CarId,
			DriverId = request.DriverId,
			PickupDestinationId = request.PickupDestinationId,
			DropoffDestinationId = request.DropoffDestinationId,
			StartTime = startTimeUtc,
			EndTime = endTimeUtc,
			Status = "INACTIVE",
			IsActive = false
		};

		await _unitOfWork.ExecuteInTransactionAsync(async () =>
		{
			await _unitOfWork.Schedules.AddAsync(schedule);
			await _unitOfWork.SaveChangeAsync();

            /*
            Giả sử mỗi xe có số lượng ghế cố định,
            khi tạo lịch chạy mới sẽ tự động tạo các bản ghi SeatBooking
            tương ứng với số ghế của xe đó, với trạng thái ban đầu là chưa được đặt
            (IsBooking = false) và không giữ chỗ (IsHold = false).
            */
			for (var seatNumber = 1; seatNumber <= carWithSeats.SeatCount; seatNumber++)
			{
				var seatBooking = new SeatBooking
				{
					ScheduleId = schedule.Id,
					SeatNumber = seatNumber,
					IsBooking = false,
					IsHold = false
				};

				await _unitOfWork.SeatBookings.AddAsync(seatBooking);
			}

			await _unitOfWork.SaveChangeAsync();
		});

		var createdSchedule = await _unitOfWork.Schedules.GetScheduleWithDetailsByIdAsync(schedule.Id);
		var summary = MapScheduleSummaryResponse(createdSchedule!);
		return ServiceResult<CreateScheduleResponse>.Success(new CreateScheduleResponse
		{
			Id = summary.Id,
			CarId = summary.CarId,
			DriverId = summary.DriverId,
			DriverName = summary.DriverName,
			PickupDestinationId = summary.PickupDestinationId,
			PickupDestinationName = summary.PickupDestinationName,
			DropoffDestinationId = summary.DropoffDestinationId,
			DropoffDestinationName = summary.DropoffDestinationName,
			StartTime = summary.StartTime,
			EndTime = summary.EndTime,
			IsActive = summary.IsActive,
			Status = summary.Status,
			TotalSeatBookings = summary.TotalSeatBookings,
			TotalBookedSeats = summary.TotalBookedSeats,
			Car = summary.Car
		});
	}

	public async Task<ServiceResult> DeleteScheduleAsync(int id)
	{
		var schedule = await _unitOfWork.Schedules.GetByIdAsync(id);
		if (schedule is null)
		{
			return ServiceResult.NotFound($"Không tìm thấy lịch chạy id={id}.");
		}

		var seatBookings = await _unitOfWork.SeatBookings.FindAsync(sb => sb.ScheduleId == id && sb.IsBooking);
		if (seatBookings.Any())
		{
			return ServiceResult.BadRequest("Không thể xoá lịch vì đã có ghế được đặt.");
		}

		_unitOfWork.Schedules.Remove(schedule);
		await _unitOfWork.CompleteAsync();
		return ServiceResult.Success();
	}

	public async Task<ServiceResult<UpdateScheduleStatusResponse>> UpdateScheduleStatusAsync(int id, UpdateScheduleStatusRequest request)
	{
		var targetStatus = request.Status.Trim().ToUpperInvariant();
		if (!AllowedStatuses.Contains(targetStatus))
		{
			return ServiceResult<UpdateScheduleStatusResponse>.BadRequest("Status không hợp lệ. Chỉ chấp nhận: ACTIVE, INACTIVE, FINISH.");
		}

		var schedule = await _unitOfWork.Schedules.GetByIdAsync(id);
		if (schedule is null)
		{
			return ServiceResult<UpdateScheduleStatusResponse>.NotFound($"Không tìm thấy lịch chạy id={id}.");
		}

		var currentStatus = schedule.Status.Trim().ToUpperInvariant();
		if (currentStatus == "FINISH" && targetStatus != "FINISH")
		{
			return ServiceResult<UpdateScheduleStatusResponse>.BadRequest("Schedule đã FINISH (terminal), không thể chuyển trạng thái khác.");
		}

		if (targetStatus == "FINISH" && currentStatus != "ACTIVE")
		{
			return ServiceResult<UpdateScheduleStatusResponse>.BadRequest("Chỉ được chuyển sang FINISH từ ACTIVE.");
		}

		if (!IsValidStatusTransition(currentStatus, targetStatus))
		{
			return ServiceResult<UpdateScheduleStatusResponse>.BadRequest($"Không thể chuyển trạng thái từ {currentStatus} sang {targetStatus}.");
		}

		schedule.Status = targetStatus;
		schedule.IsActive = targetStatus == "ACTIVE";

		_unitOfWork.Schedules.Update(schedule);
		await _unitOfWork.SaveChangeAsync();

		var updatedSchedule = await _unitOfWork.Schedules.GetScheduleWithDetailsByIdAsync(id);
		var summary = MapScheduleSummaryResponse(updatedSchedule!);

		return ServiceResult<UpdateScheduleStatusResponse>.Success(new UpdateScheduleStatusResponse
		{
			Id = summary.Id,
			CarId = summary.CarId,
			DriverId = summary.DriverId,
			DriverName = summary.DriverName,
			PickupDestinationId = summary.PickupDestinationId,
			PickupDestinationName = summary.PickupDestinationName,
			DropoffDestinationId = summary.DropoffDestinationId,
			DropoffDestinationName = summary.DropoffDestinationName,
			StartTime = summary.StartTime,
			EndTime = summary.EndTime,
			IsActive = summary.IsActive,
			Status = summary.Status,
			TotalSeatBookings = summary.TotalSeatBookings,
			TotalBookedSeats = summary.TotalBookedSeats,
			Car = summary.Car
		});
	}

	private static bool IsValidStatusTransition(string currentStatus, string targetStatus)
	{
		if (currentStatus == targetStatus)
		{
			return true;
		}

		return currentStatus switch
		{
			"INACTIVE" => targetStatus == "ACTIVE",
			"ACTIVE" => targetStatus == "INACTIVE" || targetStatus == "FINISH",
			"FINISH" => false,
			_ => false
		};
	}

	private static Task<bool> HasScheduleConflictAsync(CreateScheduleRequest request)
	{
		// TODO: BT8.1 sẽ triển khai chi tiết logic conflict theo thời gian/xe/tài xế.
		return Task.FromResult(false);
	}

	private static DateTime EnsureUtc(DateTime value)
	{
		if (value.Kind == DateTimeKind.Utc)
		{
			return value;
		}

		if (value.Kind == DateTimeKind.Local)
		{
			return value.ToUniversalTime();
		}

		return DateTime.SpecifyKind(value, DateTimeKind.Utc);
	}

	private static DateTime? EnsureUtc(DateTime? value)
	{
		if (!value.HasValue)
		{
			return null;
		}

		return EnsureUtc(value.Value);
	}

	private static ScheduleSummaryResponse MapScheduleSummaryResponse(Schedule schedule)
		=> new()
		{
			Id = schedule.Id,
			CarId = schedule.CarId,
			DriverId = schedule.DriverId,
			DriverName = schedule.Driver?.FullName ?? string.Empty,
			PickupDestinationId = schedule.PickupDestinationId,
			PickupDestinationName = schedule.PickupDestination?.Name ?? string.Empty,
			DropoffDestinationId = schedule.DropoffDestinationId,
			DropoffDestinationName = schedule.DropoffDestination?.Name ?? string.Empty,
			StartTime = schedule.StartTime,
			EndTime = schedule.EndTime,
			IsActive = schedule.IsActive,
			Status = schedule.Status,
			TotalSeatBookings = schedule.SeatBookings.Count,
			TotalBookedSeats = schedule.SeatBookings.Count(sb => sb.IsBooking),
			Car = schedule.Car is null
				? null
				: new ScheduleCarSummaryResponse
				{
					Id = schedule.Car.Id,
					LicensePlate = schedule.Car.LicensePlate,
					Brand = schedule.Car.Brand,
					Model = schedule.Car.Model,
					SeatCount = schedule.Car.SeatCount
				}
		};
}
