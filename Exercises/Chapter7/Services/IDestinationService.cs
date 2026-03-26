using Chapter6.DTOs.Destinations;
using Chapter6.Services.Common;

namespace Chapter6.Services;

public interface IDestinationService
{
    Task<ServiceResult<CreateDestinationResponse>> CreateAsync(CreateDestinationRequest request);
    Task<ServiceResult<GetDestinationByIdResponse>> GetByIdAsync(int id);
}
