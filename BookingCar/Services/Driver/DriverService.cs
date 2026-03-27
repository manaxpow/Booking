using System.Text.Json;
using Microsoft.AspNetCore.OutputCaching;
using Microsoft.Extensions.Caching.Distributed;
using VehicleBooking.Models.DTOs.Common;

public class DriverService : IDriverService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IDistributedCache _distributedCache;

    public DriverService(IUnitOfWork unitOfWork, IDistributedCache distributedCache)
    {
        _unitOfWork = unitOfWork;
        _distributedCache = distributedCache;
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
        var cacheKey = CacheKeyFactory.GetDriverKey(id);

        var driverCache = await _distributedCache.GetStringAsync(cacheKey);
        if (driverCache != null)
        {
            return JsonSerializer.Deserialize<DriverDetailResponse>(driverCache);
        }
        var driver = await _unitOfWork.Drivers.GetByIdAsync(id);
        if (driver == null) return null;

        var driverDetailResponse = new DriverDetailResponse(
            driver.Id,
            driver.Name,
            driver.Dob,
            driver.License,
            driver.CreateAt,
            driver.UpdateAt);

        var options = new DistributedCacheEntryOptions
        {
            AbsoluteExpirationRelativeToNow = TimeSpan.FromHours(24)
        };

        await _distributedCache.SetStringAsync(
            cacheKey,
            JsonSerializer.Serialize(driverDetailResponse),
            options);

        return driverDetailResponse;
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

        await _distributedCache.RemoveAsync(CacheKeyFactory.GetDriverKey(driver.Id));
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

        await _distributedCache.RemoveAsync(CacheKeyFactory.GetDriverKey(driver.Id));
    }
}
