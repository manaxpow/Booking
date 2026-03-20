using DataAccess.Repositories.Interfaces;
using DataAccess.Data;

public interface IUnitOfWork : IDisposable
{
    IGenericRepository<T> Repository<T>() where T : class;
    IUserRepository Users { get; }
    IDriverRepository Drivers { get; }
    ICarRepository Cars { get; }
    ISeatRepository Seats { get; }
    IDestinationRepository Destinations { get; }
    IScheduleRepository Schedules { get; }
    AppDbContext Context { get; }
    Task<int> CompleteAsync();
    Task SaveChangeAsync();
}