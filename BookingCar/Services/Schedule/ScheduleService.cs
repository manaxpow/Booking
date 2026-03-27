using System.Text.Json;
using Microsoft.AspNetCore.OutputCaching;
using Microsoft.Extensions.Caching.Distributed;
using Models.Dtos.Schedule;
using VehicleBooking.Models.DTOs.Common;
using ScheduleEntity = Schedule;

namespace Services.Schedule;

public class ScheduleService : IScheduleService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IOutputCacheStore _cacheStore;
    private readonly IDistributedCache _distributedCache;
    private readonly ISeatService _seatService;
    private readonly IDestinationService _destService;
    private readonly ICarService _carService;
    private readonly IDriverService _driverService;

    public ScheduleService(IUnitOfWork unitOfWork, IOutputCacheStore cacheStore, IDistributedCache distributedCache, ISeatService seatService, IDestinationService destService, ICarService carService, IDriverService driverService)
    {
        _unitOfWork = unitOfWork;
        _cacheStore = cacheStore;
        _distributedCache = distributedCache;
        _seatService = seatService;
        _destService = destService;
        _carService = carService;
        _driverService = driverService;
    }

    public async Task<PagedResult<ScheduleResponse>> GetSchedulesAsync(ScheduleQuery query)
    {

        var (schedules, totalCount) = await _unitOfWork.Schedules
            .GetPagedSchedulesAsync(query);

        var responses = schedules.Select(s =>
            new ScheduleResponse(
                s.Id,
                s.CarId,
                s.DriverId,
                s.Car.LicensePlate,
                s.Driver.Name,
                s.FromDestinationId,
                s.FromDestination.Province,
                s.ToDestinationId,
                s.ToDestination.Province,
                s.StartTime,
                s.EndTime,
                s.ExpectedDuration,
                s.Status,
                s.Price,
                s.CreateAt,
                s.UpdateAt
            )).ToList();

        return new PagedResult<ScheduleResponse>
        {
            Items = responses,
            TotalCount = totalCount,
            Page = query.Page,
            PageSize = query.PageSize
        };
    }

    public async Task<ScheduleResponse?> GetScheduleByIdAsync(int id)
    {
        var key = CacheKeyFactory.GetScheduleKey(id);

        var rawData = await _distributedCache.GetStringAsync(key);
        ScheduleEntity? schedule;
        if (rawData != null)
        {
            schedule = JsonSerializer.Deserialize<ScheduleEntity>(rawData);
        }
        else
        {
            schedule = await _unitOfWork.Schedules.GetByIdAsync(id);
            if (schedule == null) throw new KeyNotFoundException("Không tìm thay lịch trình.");
            var options = new DistributedCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(10)
            };
            await _distributedCache.SetStringAsync(key, JsonSerializer.Serialize(schedule), options);
        }

        var car = await _carService.GetCarByIdAsync(schedule!.CarId);
        var driver = await _driverService.GetDriverByIdAsync(schedule.DriverId);
        var fromDest = await _destService.GetDestinationByIdAsync(schedule.FromDestinationId);
        var toDest = await _destService.GetDestinationByIdAsync(schedule.ToDestinationId);

        return new ScheduleResponse(
            schedule.Id,
            car?.Id ?? 0,
            driver?.Id ?? 0,
            car?.LicensePlate ?? "",
            driver?.Name ?? "",
            fromDest?.Id ?? 0,
            fromDest?.Province ?? "",
            toDest?.Id ?? 0,
            toDest?.Province ?? "",
            schedule.StartTime,
            schedule.EndTime,
            schedule.ExpectedDuration,
            schedule.Status,
            schedule.Price,
            schedule.CreateAt,
            schedule.UpdateAt
        );
    }

    public async Task CreateScheduleAsync(CreateScheduleRequest request)
    {
        // Validate Car exists
        var car = await _unitOfWork.Cars.GetByIdAsync(request.CarId);
        if (car == null)
            throw new KeyNotFoundException("Không tìm thấy xe.");

        // Validate Driver exists
        var driver = await _unitOfWork.Drivers.GetByIdAsync(request.DriverId);
        if (driver == null)
            throw new KeyNotFoundException("Không tìm thấy tài xế.");

        // Validate Destinations exist
        var fromDestination = await _unitOfWork.Destinations.GetByIdAsync(request.FromDestinationId);
        if (fromDestination == null)
            throw new KeyNotFoundException("Không tìm thấy điểm đi.");

        var toDestination = await _unitOfWork.Destinations.GetByIdAsync(request.ToDestinationId);
        if (toDestination == null)
            throw new KeyNotFoundException("Không tìm thấy điểm đến.");

        var startTimeUtc = DateTime.SpecifyKind(request.StartTime, DateTimeKind.Utc);

        // Check for active schedule conflict
        var hasActiveCarSchedule = await _unitOfWork.Schedules.HasActiveCarScheduleAsync(
            request.CarId, startTimeUtc, request.ExpectedDuration);
        var hasActiveDriverSchedule = await _unitOfWork.Schedules.HasActiveDriverScheduleAsync(
            request.DriverId, startTimeUtc, request.ExpectedDuration);
        if (hasActiveCarSchedule || hasActiveDriverSchedule)
            throw new InvalidOperationException("Xe hoặc tài xế này đang có lịch trình trong khoảng thời gian đã chọn.");

        var schedule = new ScheduleEntity
        {
            CarId = request.CarId,
            DriverId = request.DriverId,
            FromDestinationId = request.FromDestinationId,
            ToDestinationId = request.ToDestinationId,
            StartTime = startTimeUtc,
            EndTime = startTimeUtc.Add(request.ExpectedDuration),
            Status = request.Status,
            Price = request.Price,
            CreateAt = DateTime.UtcNow,
            UpdateAt = DateTime.UtcNow,
            ExpectedDuration = request.ExpectedDuration,
            SeatBookings = new List<SeatBooking>()
        };

        // Get car with seats
        var carWithSeats = await _unitOfWork.Cars.GetCarWithSeatsByIdAsync(request.CarId);
        if (carWithSeats == null)
            throw new KeyNotFoundException("Không tìm thấy xe.");

        // Create SeatBookings for all seats
        foreach (var seat in carWithSeats.Seats)
        {
            var seatBooking = new SeatBooking
            {
                SeatId = seat.Id,
                IsBooking = false,
                IsHold = false
            };
            schedule.SeatBookings.Add(seatBooking);
        }

        await _unitOfWork.Schedules.AddAsync(schedule);
        await _unitOfWork.SaveChangeAsync();
    }

    public async Task UpdateScheduleAsync(UpdateScheduleRequest request)
    {
        var schedule = await _unitOfWork.Schedules
            .GetScheduleWithDetailsByIdAsync(request.Id);

        if (schedule == null)
            throw new KeyNotFoundException("Không tìm thấy lịch trình.");

        // validate
        var newCarId = request.CarId ?? schedule.CarId;
        var newDriverId = request.DriverId ?? schedule.DriverId;
        var newStartTime = request.StartTime ?? schedule.StartTime;

        if (request.CarId.HasValue)
            await ValidateCarExists(request.CarId.Value);

        if (request.DriverId.HasValue)
            await ValidateDriverExists(request.DriverId.Value);

        if (request.FromDestinationId.HasValue)
            await ValidateDestinationExists(request.FromDestinationId.Value, "điểm đi");

        if (request.ToDestinationId.HasValue)
            await ValidateDestinationExists(request.ToDestinationId.Value, "điểm đến");


        // check for schedule conflicts if car, driver, or start time is changing
        if (schedule.StartTime != newStartTime ||
            schedule.CarId != newCarId ||
            schedule.DriverId != newDriverId)
        {
            var carConflictTask = await _unitOfWork.Schedules
                .HasActiveCarScheduleAsync(newCarId, newStartTime, schedule.ExpectedDuration);

            var driverConflictTask = await _unitOfWork.Schedules
                .HasActiveDriverScheduleAsync(newDriverId, newStartTime, schedule.ExpectedDuration);


            if (carConflictTask || driverConflictTask)
                throw new InvalidOperationException("Xe hoặc tài xế đã có lịch.");
        }


        if (schedule.CarId != newCarId || schedule.StartTime != newStartTime)
        {
            _unitOfWork.Context.SeatBookings.RemoveRange(schedule.SeatBookings);

            var car = await _unitOfWork.Cars.GetCarWithSeatsByIdAsync(newCarId);
            if (car == null)
                throw new KeyNotFoundException("Không tìm thấy xe.");

            schedule.SeatBookings = car.Seats.Select(seat => new SeatBooking
            {
                SeatId = seat.Id,
                IsBooking = false,
                IsHold = false
            }).ToList();
        }

        // Update fields
        schedule.CarId = newCarId;
        schedule.DriverId = newDriverId;
        schedule.StartTime = newStartTime;

        if (request.FromDestinationId.HasValue)
            schedule.FromDestinationId = request.FromDestinationId.Value;

        if (request.ToDestinationId.HasValue)
            schedule.ToDestinationId = request.ToDestinationId.Value;

        if (!string.IsNullOrEmpty(request.Status))
            schedule.Status = request.Status;

        if (request.Price.HasValue)
            schedule.Price = request.Price.Value;

        schedule.UpdateAt = DateTime.UtcNow;

        _unitOfWork.Schedules.Update(schedule);
        await _unitOfWork.SaveChangeAsync();

        // Evict cache
        await _distributedCache.RemoveAsync(CacheKeyFactory.GetScheduleKey(schedule.Id));
    }
    public async Task DeleteScheduleAsync(int id)
    {
        var schedule = await _unitOfWork.Schedules.GetScheduleWithDetailsByIdAsync(id);
        if (schedule == null)
            throw new KeyNotFoundException("Không tìm thấy lịch trình.");

        // Check if schedule has bookings
        if (schedule.SeatBookings.Any(sb => sb.IsBooking))
            throw new InvalidOperationException("Lịch trình này đang có vé đã được đặt, không thể xóa.");

        _unitOfWork.Schedules.Remove(schedule);
        await _unitOfWork.SaveChangeAsync();

        // Evict cache
        await _distributedCache.RemoveAsync(CacheKeyFactory.GetScheduleKey(schedule.Id));
    }

    public async Task UpdateScheduleStatusAsync(int id, string status)
    {
        var schedule = await _unitOfWork.Schedules.GetScheduleWithDetailsByIdAsync(id);
        if (schedule == null)
            throw new KeyNotFoundException("Không tìm thấy lịch trình.");

        var validStatuses = new[] { "ACTIVE", "INACTIVE", "FINISH" };
        if (!validStatuses.Contains(status))
            throw new ArgumentException("Trạng thái không hợp lệ.");

        // Validate status transitions
        if (schedule.Status == "FINISH")
            throw new InvalidOperationException("Lịch trình đã hoàn thành, không thể thay đổi trạng thái.");

        if (status == "FINISH" && schedule.Status != "ACTIVE")
            throw new InvalidOperationException("Chỉ có thể hoàn thành lịch trình đang hoạt động.");

        schedule.Status = status;
        schedule.UpdateAt = DateTime.UtcNow;

        _unitOfWork.Schedules.Update(schedule);
        await _unitOfWork.SaveChangeAsync();

        // Evict cache
        await _distributedCache.RemoveAsync(CacheKeyFactory.GetScheduleKey(schedule.Id));
    }

    public async Task<PagedResult<SeatBookingResponse>> GetSeatBookingByScheduleIdAsync(int scheduleId)
    {
        var seatBookings = await _unitOfWork.Schedules.GetSeatBookingByScheduleIdAsync(scheduleId);

        var response = new PagedResult<SeatBookingResponse>
        {
            Items = seatBookings.Select(sb => new SeatBookingResponse
            (
                sb.Id,
                sb.SeatId,
                sb.Seat.Name,
                sb.ScheduleId,
                sb.IsHold,
                sb.IsBooking,
                sb.CreateAt,
                sb.UpdateAt
            )).ToList(),

            TotalCount = seatBookings.Count(),
            Page = 1,
            PageSize = seatBookings.Count()
        };
        return response;
    }

    private async Task ValidateCarExists(int carId)
    {
        if (await _unitOfWork.Cars.GetByIdAsync(carId) == null)
            throw new KeyNotFoundException("Không tìm thấy xe.");
    }

    private async Task ValidateDriverExists(int driverId)
    {
        if (await _unitOfWork.Drivers.GetByIdAsync(driverId) == null)
            throw new KeyNotFoundException("Không tìm thấy tài xế.");
    }

    private async Task ValidateDestinationExists(int id, string type)
    {
        if (await _unitOfWork.Destinations.GetByIdAsync(id) == null)
            throw new KeyNotFoundException($"Không tìm thấy {type}.");
    }
}