using Chapter6.DTOs.Cars;
using Chapter6.Services.Common;

namespace Chapter6.Services;

public interface ICarService
{
    Task<ServiceResult<IEnumerable<CarSummaryResponse>>> GetAllAsync();
    Task<ServiceResult<GetCarByIdResponse>> GetByIdAsync(int id);
    Task<ServiceResult<CreateCarResponse>> CreateAsync(CreateCarRequest request);
    Task<ServiceResult> DeleteAsync(int id);
}
