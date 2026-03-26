using Chapter6.DTOs.Cars;
using Chapter6.Services.Common;

namespace Chapter6.Services;

public interface ICarService
{
	Task<ServiceResult<GetCarByIdResponse>> GetByIdAsync(int id);
	Task<ServiceResult<CreateCarResponse>> CreateAsync(CreateCarRequest request);
}
