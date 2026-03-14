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
}