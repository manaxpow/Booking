using DataAccess.Data;
using Microsoft.EntityFrameworkCore;

public class SeatRepository : GenericRepository<Seat>, ISeatRepository
{
    public SeatRepository(AppDbContext context) : base(context)
    {
    }

    public Task<Seat?> GetByCarIdAndNameAsync(int carId, string name)
    {
        return _context.Seats.FirstOrDefaultAsync(s => s.CarId == carId && s.Name == name);
    }

    public async Task<Seat?> GetByNameAsync(string name)
    {
        return await _context.Seats.FirstOrDefaultAsync(s => s.Name == name);
    }
}