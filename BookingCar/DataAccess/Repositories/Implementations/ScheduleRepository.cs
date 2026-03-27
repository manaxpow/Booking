using DataAccess.Data;
using Microsoft.EntityFrameworkCore;
using Models.Dtos.Schedule;
using DataAccess.Repositories.Interfaces;
using Microsoft.AspNetCore.OutputCaching;

namespace DataAccess.Repositories.Implementations;

public class ScheduleRepository : GenericRepository<Schedule>, IScheduleRepository
{

    public ScheduleRepository(AppDbContext context) : base(context)
    {
    }

    public async Task<Schedule?> GetScheduleWithDetailsByIdAsync(int id)
    {
        return await _dbSet
            .Include(s => s.Car)
            .Include(s => s.Driver)
            .Include(s => s.FromDestination)
            .Include(s => s.ToDestination)
            .Include(s => s.SeatBookings)
                .ThenInclude(sb => sb.Seat)
            .AsSplitQuery()
            .FirstOrDefaultAsync(s => s.Id == id);
    }

    public async Task<(IEnumerable<Schedule> Schedules, int TotalCount)> GetPagedSchedulesAsync(ScheduleQuery query)
    {
        var scheduleQuery = _dbSet
            .Include(s => s.Car)
            .Include(s => s.Driver)
            .Include(s => s.FromDestination)
            .Include(s => s.ToDestination)
            .AsNoTracking()
            .AsQueryable();

        if (query.CarId.HasValue)
        {
            scheduleQuery = scheduleQuery.Where(s => s.CarId == query.CarId.Value);
        }

        if (query.DriverId.HasValue)
        {
            scheduleQuery = scheduleQuery.Where(s => s.DriverId == query.DriverId.Value);
        }

        if (query.FromDestinationId.HasValue)
        {
            scheduleQuery = scheduleQuery.Where(s => s.FromDestinationId == query.FromDestinationId.Value);
        }

        if (query.ToDestinationId.HasValue)
        {
            scheduleQuery = scheduleQuery.Where(s => s.ToDestinationId == query.ToDestinationId.Value);
        }

        if (query.StartTimeFrom.HasValue)
        {
            scheduleQuery = scheduleQuery.Where(s => s.StartTime >= query.StartTimeFrom.Value);
        }

        if (query.StartTimeTo.HasValue)
        {
            scheduleQuery = scheduleQuery.Where(s => s.StartTime <= query.StartTimeTo.Value);
        }

        if (!string.IsNullOrWhiteSpace(query.Status))
        {
            scheduleQuery = scheduleQuery.Where(s => s.Status == query.Status);
        }

        int totalCount = await scheduleQuery.CountAsync();

        // Sorting
        if (!string.IsNullOrWhiteSpace(query.SortBy))
        {
            scheduleQuery = query.SortBy switch
            {
                "StartTime" => query.SortDescending
                    ? scheduleQuery.OrderByDescending(s => s.StartTime)
                    : scheduleQuery.OrderBy(s => s.StartTime),
                "Price" => query.SortDescending
                    ? scheduleQuery.OrderByDescending(s => s.Price)
                    : scheduleQuery.OrderBy(s => s.Price),
                _ => scheduleQuery.OrderByDescending(s => s.StartTime)
            };
        }
        else
        {
            scheduleQuery = scheduleQuery.OrderByDescending(s => s.StartTime);
        }

        var schedules = await scheduleQuery
            .Skip((query.Page - 1) * query.PageSize)
            .Take(query.PageSize)
            .ToListAsync();

        return (schedules, totalCount);
    }

    public async Task<IEnumerable<Schedule>> GetSchedulesByCarIdAsync(int carId)
    {
        return await _dbSet
            .Where(s => s.CarId == carId && s.Status == "ACTIVE")
            .OrderBy(s => s.StartTime)
            .ToListAsync();
    }

    public async Task<IEnumerable<Schedule>> GetSchedulesByDriverIdAsync(int driverId)
    {
        return await _dbSet
            .Where(s => s.DriverId == driverId && s.Status == "ACTIVE")
            .OrderBy(s => s.StartTime)
            .ToListAsync();
    }

    public async Task<bool> HasActiveCarScheduleAsync(int carId, DateTime startTime, TimeSpan duration)
    {
        startTime = DateTime.SpecifyKind(startTime, DateTimeKind.Utc);

        var endTime = startTime.Add(duration);

        return await _dbSet.AnyAsync(s =>
            s.CarId == carId &&
            s.Status == "ACTIVE" &&
            s.StartTime < endTime.AddMinutes(60) &&
            s.StartTime > startTime.AddMinutes(-60) // cach nhau it nhat 1h de tranh truong hop lich trinh ke tiep nhau gan nhau
        );
    }
    public async Task<bool> HasActiveDriverScheduleAsync(int driverId, DateTime startTime, TimeSpan expectedDuration)
    {
        startTime = DateTime.SpecifyKind(startTime, DateTimeKind.Utc);
        var endTime = startTime.Add(expectedDuration);

        return await _dbSet.AnyAsync(s =>
            s.DriverId == driverId &&
            s.Status == "ACTIVE" &&
            s.StartTime < endTime.AddMinutes(60) &&
            s.StartTime > startTime.AddMinutes(-60) // cach nhau it nhat 1h de tranh truong hop lich trinh ke tiep nhau gan nhau
        );

    }

    public async Task<IEnumerable<SeatBooking>> GetSeatBookingByScheduleIdAsync(int scheduleId)
    {
        return await _dbSet
            .Where(s => s.Id == scheduleId)
            .SelectMany(s => s.SeatBookings)
            .Include(sb => sb.Seat)
            .ToListAsync();
    }

    public async Task<SeatBooking?> GetSeatForUpdateAsync(int SeatBookingId)
    {
        return await _context.SeatBookings
        .FromSqlRaw("SELECT * FROM \"SeatBookings\" WHERE \"Id\" = {0} FOR UPDATE", SeatBookingId)
        .Include(sb => sb.Schedule)
        .Include(sb => sb.Seat)
        .FirstOrDefaultAsync();
    }
}