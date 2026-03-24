using Chapter6.DataAccess.UnitOfWork;
using Chapter6.DTOs.Cars;
using Chapter6.Models;
using Chapter6.Services.Common;

namespace Chapter6.Services;

public class CarService : ICarService
{
	private readonly IUnitOfWork _unitOfWork;

    public CarService(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

	public async Task<ServiceResult<GetCarByIdResponse>> GetByIdAsync(int id)
	{
		var car = await _unitOfWork.Cars.GetByIdAsync(id);
		if (car is null)
		{
			return ServiceResult<GetCarByIdResponse>.NotFound($"Không tìm thấy xe id={id}.");
		}

		var summary = MapCarSummaryResponse(car);
		return ServiceResult<GetCarByIdResponse>.Success(new GetCarByIdResponse
		{
			Id = summary.Id,
			LicensePlate = summary.LicensePlate,
			Brand = summary.Brand,
			Model = summary.Model,
			SeatCount = summary.SeatCount,
			Color = summary.Color,
			ManufactureYear = summary.ManufactureYear,
			IsActive = summary.IsActive,
			CreatedAt = summary.CreatedAt
		});
	}

	public async Task<ServiceResult<CreateCarResponse>> CreateAsync(CreateCarRequest request)
	{
		var existed = await _unitOfWork.Cars.GetByLicensePlateAsync(request.LicensePlate);
		if (existed is not null)
		{
			return ServiceResult<CreateCarResponse>.Conflict($"Biển số '{request.LicensePlate}' đã tồn tại.");
		}

		var car = new Car
		{
			LicensePlate = request.LicensePlate.Trim(),
			Brand = request.Brand.Trim(),
			Model = request.Model.Trim(),
			SeatCount = request.SeatCount,
			Color = request.Color?.Trim(),
			ManufactureYear = request.ManufactureYear,
			IsActive = true,
			CreatedAt = DateTime.UtcNow
		};

		await _unitOfWork.Cars.AddAsync(car);
		await _unitOfWork.CompleteAsync();

		var summary = MapCarSummaryResponse(car);
		return ServiceResult<CreateCarResponse>.Success(new CreateCarResponse
		{
			Id = summary.Id,
			LicensePlate = summary.LicensePlate,
			Brand = summary.Brand,
			Model = summary.Model,
			SeatCount = summary.SeatCount,
			Color = summary.Color,
			ManufactureYear = summary.ManufactureYear,
			IsActive = summary.IsActive,
			CreatedAt = summary.CreatedAt
		});
	}

	public async Task<ServiceResult> DeleteAsync(int id)
	{
		var car = await _unitOfWork.Cars.GetByIdAsync(id);
		if (car is null)
		{
			return ServiceResult.NotFound($"Không tìm thấy xe id={id}.");
		}

		var hasActiveSchedule = await _unitOfWork.Schedules.HasActiveCarScheduleAsync(id);
		if (hasActiveSchedule)
		{
			return ServiceResult.BadRequest("Xe đang có lịch hoạt động, không thể xoá.");
		}

		_unitOfWork.Cars.Remove(car);
		await _unitOfWork.CompleteAsync();
		return ServiceResult.Success();
	}

	private static CarSummaryResponse MapCarSummaryResponse(Car car)
		=> new()
		{
			Id = car.Id,
			LicensePlate = car.LicensePlate,
			Brand = car.Brand,
			Model = car.Model,
			SeatCount = car.SeatCount,
			Color = car.Color,
			ManufactureYear = car.ManufactureYear,
			IsActive = car.IsActive,
			CreatedAt = car.CreatedAt
		};
}
