using Chapter6.Models;

namespace Chapter6.DataAccess.Repositories;

public interface IScheduleRepository : IGenericRepository<Schedule>
{
    Task<Schedule?> GetScheduleWithDetailsByIdAsync(int id);
    Task<IEnumerable<Schedule>> GetPagedSchedulesAsync(
        string? driverName,
        DateTime? fromDate,
        DateTime? toDate,
        int page,
        int pageSize
    );
    Task<bool> HasActiveCarScheduleAsync(int carId);
    Task<bool> HasActiveDriverScheduleAsync(int driverId);
}
