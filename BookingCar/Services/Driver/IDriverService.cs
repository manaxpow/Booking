using VehicleBooking.Models.DTOs.Common;

public interface IDriverService
{
    Task<PagedResult<DriverResponse>> GetDriversAsync(DriverQueryParameters query);
    Task<DriverDetailResponse?> GetDriverByIdAsync(int id);
    Task CreateDriverAsync(CreateDriverRequest request);
    Task UpdateDriverAsync(UpdateDriverRequest request);
    Task DeleteDriverAsync(int id);
}
