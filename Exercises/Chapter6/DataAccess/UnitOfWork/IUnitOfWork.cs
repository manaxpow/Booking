using Chapter6.DataAccess.Repositories;
using Chapter6.Models;

namespace Chapter6.DataAccess.UnitOfWork;

public interface IUnitOfWork
{
	ICarRepository Cars { get; }
	IScheduleRepository Schedules { get; }
	IDriverRepository Drivers { get; }
	IGenericRepository<Destination> Destinations { get; }
	IGenericRepository<SeatBooking> SeatBookings { get; }

	Task<int> CompleteAsync();
	Task<int> SaveChangeAsync();
	Task ExecuteInTransactionAsync(Func<Task> action);
}
