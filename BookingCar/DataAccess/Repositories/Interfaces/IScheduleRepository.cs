using DataAccess.Repositories.Interfaces;
using Models.Dtos.Schedule;

namespace DataAccess.Repositories.Interfaces;

public interface IScheduleRepository : IGenericRepository<Schedule>
{
    Task<Schedule?> GetScheduleWithDetailsByIdAsync(int id);
    Task<(IEnumerable<Schedule> Schedules, int TotalCount)> GetPagedSchedulesAsync(ScheduleQuery query);
    Task<IEnumerable<Schedule>> GetSchedulesByCarIdAsync(int carId);
    Task<IEnumerable<Schedule>> GetSchedulesByDriverIdAsync(int driverId);
    Task<IEnumerable<SeatBooking>> GetSeatBookingByScheduleIdAsync(int scheduleId);
    Task<SeatBooking?> GetSeatForUpdateAsync(int SeatBookingId);
    Task<bool> HasActiveDriverScheduleAsync(int driverId, DateTime startTime, TimeSpan expectedDuration);
    Task<bool> HasActiveCarScheduleAsync(int carId, DateTime startTime, TimeSpan duration);

}