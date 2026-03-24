using Chapter6.DTOs.Drivers;
using Chapter6.Services.Common;

public interface IDriverService
{
    Task<ServiceResult<CreateDriverResponse>> CreateAsync(CreateDriverRequest request);
    Task<ServiceResult<GetDriverByIdResponse>> GetByIdAsync(int id);
}