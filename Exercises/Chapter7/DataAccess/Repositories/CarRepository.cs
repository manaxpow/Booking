using Chapter6.DataAccess.Context;
using Chapter6.Models;
using Microsoft.EntityFrameworkCore;

namespace Chapter6.DataAccess.Repositories;

public class CarRepository : GenericRepository<Car>, ICarRepository
{
    public CarRepository(AppDbContext context)
        : base(context) { }

    public async Task<Car?> GetByLicensePlateAsync(string licensePlate) =>
        await _context.Cars.FirstOrDefaultAsync(c => c.LicensePlate == licensePlate);

    public async Task<Car?> GetCarWithSeatsByIdAsync(int id) =>
        await _context.Cars.FirstOrDefaultAsync(c => c.Id == id);

    public async Task<IEnumerable<Car>> GetPagedCarsAsync(
        string? keyword,
        string? brand,
        int page,
        int pageSize
    )
    {
        var query = _context.Cars.AsQueryable();

        if (!string.IsNullOrWhiteSpace(keyword))
        {
            query = query.Where(c => c.LicensePlate.Contains(keyword) || c.Model.Contains(keyword));
        }

        if (!string.IsNullOrWhiteSpace(brand))
        {
            query = query.Where(c => c.Brand == brand);
        }

        return await query
            .OrderBy(c => c.Id)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();
    }
}
