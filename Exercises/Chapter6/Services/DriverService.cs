using Chapter6.DataAccess.Repositories;
using Chapter6.DataAccess.UnitOfWork;
using Chapter6.DTOs.Drivers;
using Chapter6.Models;
using Chapter6.Services.Common;

public class DriverService : IDriverService
{
    private readonly IUnitOfWork _unitOfWork;

    public DriverService(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<ServiceResult<CreateDriverResponse>> CreateAsync(CreateDriverRequest request)
    {
        var driver = new Driver
        {
            FullName = request.FullName,
        };

        await _unitOfWork.Drivers.AddAsync(driver);
        await _unitOfWork.SaveChangeAsync();

        return ServiceResult<CreateDriverResponse>.Success(new CreateDriverResponse { Id = driver.Id });
    }

    public async Task<ServiceResult<GetDriverByIdResponse>> GetByIdAsync(int id)
    {
        var driver = await _unitOfWork.Drivers.GetByIdAsync(id);
        if (driver is null)
        {
            return ServiceResult<GetDriverByIdResponse>.NotFound("Driver not found.");
        }

        return ServiceResult<GetDriverByIdResponse>.Success(new GetDriverByIdResponse
        {
            Id = driver.Id,
            FullName = driver.FullName,
            IsActive = driver.IsActive
        });
    }
}