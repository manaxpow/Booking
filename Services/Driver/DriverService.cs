using VehicleBooking.Models.DTOs.Common;

public class DriverService : IDriverService
{
    private readonly IUnitOfWork _unitOfWork;

    public DriverService(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<PagedResult<DriverResponse>> GetDriversAsync(DriverQueryParameters query)
    {
        var (drivers, totalCount) = await _unitOfWork.Drivers.GetPagedDriversAsync(
            query.Keyword,
            query.License,
            query.DobFrom,
            query.DobTo,
            query.Page,
            query.PageSize);

        var items = drivers.Select(d => new DriverResponse(
            d.Id,
            d.Name,
            d.Dob,
            d.License,
            d.CreateAt,
            d.UpdateAt)).ToList();

        return new PagedResult<DriverResponse>
        {
            Items = items,
            TotalCount = totalCount,
            Page = query.Page,
            PageSize = query.PageSize
        };
    }

    public async Task<DriverDetailResponse?> GetDriverByIdAsync(int id)
    {
        var driver = await _unitOfWork.Drivers.GetByIdAsync(id);
        if (driver == null) return null;

        return new DriverDetailResponse(
            driver.Id,
            driver.Name,
            driver.Dob,
            driver.License,
            driver.CreateAt,
            driver.UpdateAt);
    }

    public async Task CreateDriverAsync(CreateDriverRequest request)
    {
        var driver = new Driver
        {
            Name = request.Name,
            Dob = DateTime.SpecifyKind(request.Dob.Date, DateTimeKind.Utc),
            License = request.License,
            CreateAt = DateTime.UtcNow,
            UpdateAt = DateTime.UtcNow
        };

        await _unitOfWork.Drivers.AddAsync(driver);
        await _unitOfWork.CompleteAsync();
    }

    public async Task UpdateDriverAsync(UpdateDriverRequest request)
    {
        var driver = await _unitOfWork.Drivers.GetByIdAsync(request.Id);
        if (driver == null)
        {
            throw new Exception("Driver not found.");
        }

        driver.Name = request.Name;
        driver.Dob = DateTime.SpecifyKind(request.Dob.Date, DateTimeKind.Utc);
        driver.License = request.License;
        driver.UpdateAt = DateTime.UtcNow;

        _unitOfWork.Drivers.Update(driver);
        await _unitOfWork.CompleteAsync();
    }

    public async Task DeleteDriverAsync(int id)
    {
        var driver = await _unitOfWork.Drivers.GetByIdAsync(id);
        if (driver == null)
        {
            throw new Exception("Driver not found.");
        }

        _unitOfWork.Drivers.Remove(driver);
        await _unitOfWork.CompleteAsync();
    }
}
