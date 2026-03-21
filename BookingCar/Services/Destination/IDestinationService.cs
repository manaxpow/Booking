using Models.Dtos.Destination;
using VehicleBooking.Models.DTOs.Common;

public interface IDestinationService
{
    Task<PagedResult<DestinationResponse>> GetDestinationsAsync(DestinationQuery query);
    Task<DestinationResponse?> GetDestinationByIdAsync(int id);
    Task CreateDestinationAsync(CreateDestinationRequest request);
    Task UpdateDestinationAsync(UpdateDestinationRequest request,int id);
    Task DeleteDestinationAsync(int id);
}