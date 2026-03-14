using VehicleBooking.Models.DTOs.Car;
using VehicleBooking.Models.DTOs.Common;

public interface ICarService
{
    Task<PagedResult<CarResponse>> GetCarsAsync(CarQueryParameters query);
    Task<CarDetailResponse?> GetCarByIdAsync(int id);
    Task CreateCarAsync(CreateCarRequest request);
    Task UpdateCarAsync(UpdateCarRequest request);
    Task DeleteCarAsync(int id);
}