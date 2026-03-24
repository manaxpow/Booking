using Chapter6.DataAccess.Context;
using Chapter6.Models;
using Microsoft.EntityFrameworkCore;

namespace Chapter6.DataAccess.Repositories;

public class ScheduleRepository : GenericRepository<Schedule>, IScheduleRepository
{
    public ScheduleRepository(AppDbContext context)
        : base(context) { }

    public async Task<Schedule?> GetScheduleWithDetailsByIdAsync(int id) =>
        await _context
            .Schedules.Include(s => s.Car)
            .Include(s => s.Driver)
            .Include(s => s.PickupDestination)
            .Include(s => s.DropoffDestination)
            .Include(s => s.SeatBookings)
            .FirstOrDefaultAsync(s => s.Id == id);

    public async Task<IEnumerable<Schedule>> GetPagedSchedulesAsync(
        string? driverName,
        DateTime? fromDate,
        DateTime? toDate,
        int page,
        int pageSize
    )
    {
        var query = _context
            .Schedules.Include(s => s.Car)
            .Include(s => s.Driver)
            .Include(s => s.PickupDestination)
            .Include(s => s.DropoffDestination)
            .Include(s => s.SeatBookings)
            .AsQueryable();

        if (!string.IsNullOrWhiteSpace(driverName))
        {
            query = query.Where(s => s.Driver != null && s.Driver.FullName.Contains(driverName));
        }

        if (fromDate.HasValue)
        {
            query = query.Where(s => s.StartTime >= fromDate.Value);
        }

        if (toDate.HasValue)
        {
            query = query.Where(s => s.EndTime <= toDate.Value);
        }

        return await query
            .OrderByDescending(s => s.StartTime)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();
    }

    public async Task<bool> HasActiveCarScheduleAsync(int carId) =>
        await _context.Schedules.AnyAsync(s => s.CarId == carId && s.Status == "ACTIVE");

    public async Task<bool> HasActiveDriverScheduleAsync(int driverId) =>
        await _context.Schedules.AnyAsync(s => s.DriverId == driverId && s.Status == "ACTIVE");

}
