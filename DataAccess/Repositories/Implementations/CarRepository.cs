using DataAccess.Data;
using Microsoft.EntityFrameworkCore;

public class CarRepository : GenericRepository<Car>, ICarRepository
{
    public CarRepository(AppDbContext context) : base(context)
    {
    }

    public async Task<Car?> GetByLicensePlateAsync(string licensePlate)
    {
        return await _dbSet.FirstOrDefaultAsync(c => c.LicensePlate == licensePlate);
    }

    public async Task<Car?> GetCarWithSeatsByIdAsync(int id)
    {
        return await _dbSet.Include(c => c.Seats).FirstOrDefaultAsync(c => c.Id == id);
    }

    public async Task<(IEnumerable<Car> Cars, int TotalCount)> GetPagedCarsAsync(string? keyword, string? brand, int page, int pageSize)
    {
        var query = _dbSet.AsNoTracking().AsQueryable();

        if (!string.IsNullOrWhiteSpace(keyword))
        {
            query = query.Where(c => c.LicensePlate.Contains(keyword));
        }

        if (!string.IsNullOrWhiteSpace(brand))
        {
            query = query.Where(c => c.Brand.Contains(brand));
        }

        int totalCount = await query.CountAsync();
        
        var cars = await query.OrderByDescending(c => c.Id)
                              .Skip((page - 1) * pageSize)
                              .Take(pageSize)
                              .ToListAsync();

        return (cars, totalCount);
    }
}